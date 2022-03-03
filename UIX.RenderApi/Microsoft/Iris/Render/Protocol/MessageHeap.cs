// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.MessageHeap
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocol
{
    internal class MessageHeap : RenderObject
    {
        private MessageHeap.BlockInfo headBlock;
        private MessageHeap.BlockInfo tailBlock;
        private uint nextBlockId;
        private uint smallBlockSize;
        private uint blockHeaderSize;
        private uint allocExtraSize;
        private uint key;

        public MessageHeap(uint blockSize, uint headerSize, uint allocationExtra)
        {
            Debug2.Validate(blockSize > headerSize, typeof(ArgumentException), "Blocks must be larger than their headers");
            this.smallBlockSize = blockSize;
            this.blockHeaderSize = headerSize;
            this.allocExtraSize = allocationExtra;
            this.nextBlockId = 1U;
            this.CreateBlock(BlockType.First, this.smallBlockSize);
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (this.IsDisposed)
                    return;
                this.ReleaseAll(true);
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        internal bool IsDisposed => this.headBlock == null;

        public unsafe void* Alloc(uint size)
        {
            size += this.allocExtraSize;
            uint num = (uint)(sizeof(void*) - 1);
            size = (uint)((int)size + (int)num & ~(int)num);
            MessageHeap.BlockInfo block = this.tailBlock;
            if (size > block.size - block.used)
            {
                MessageHeap.BlockType type = BlockType.Normal;
                uint actualSize = this.smallBlockSize;
                if (size > this.smallBlockSize - this.blockHeaderSize)
                {
                    type = BlockType.Large;
                    actualSize = this.blockHeaderSize + size;
                }
                block = this.CreateBlock(type, actualSize);
            }
            this.ZeroMemoryBlock(block, block.used, this.allocExtraSize);
            void* voidPtr = (void*)((int)block.data + ((int)block.used + (int)this.allocExtraSize));
            block.used += size;
            ++this.key;
            return voidPtr;
        }

        public unsafe MessageHeap.Block LookupBlock(void* p)
        {
            MessageHeap.BlockInfo blockInfo = this.LookupBlockWorker(p);
            return new MessageHeap.Block(blockInfo.id, blockInfo.data, blockInfo.used);
        }

        public void Reset() => this.ReleaseAll(false);

        public int BlockCount
        {
            get
            {
                int num = 0;
                MessageHeap.BlockInfo blockInfo = this.headBlock;
                while (blockInfo != null)
                {
                    blockInfo = blockInfo.next;
                    ++num;
                }
                return num;
            }
        }

        public MessageHeap.Enumerator GetEnumerator() => new MessageHeap.Enumerator(this);

        private unsafe MessageHeap.BlockInfo CreateBlock(
          MessageHeap.BlockType type,
          uint actualSize)
        {
            ++this.key;
            MessageHeap.BlockInfo block = new MessageHeap.BlockInfo();
            block.data = Marshal.AllocCoTaskMem((int)actualSize).ToPointer();
            block.id = this.nextBlockId++;
            block.type = type;
            block.size = actualSize;
            switch (type)
            {
                case BlockType.First:
                    block.next = null;
                    this.headBlock = block;
                    this.tailBlock = block;
                    break;
                case BlockType.Normal:
                case BlockType.Large:
                    block.next = this.tailBlock.next;
                    this.tailBlock.next = block;
                    this.tailBlock = block;
                    break;
            }
            block.used = this.blockHeaderSize;
            this.ZeroMemoryBlock(block, 0U, this.blockHeaderSize);
            return block;
        }

        private unsafe void DestroyBlock(MessageHeap.BlockInfo block)
        {
            Marshal.FreeCoTaskMem(new IntPtr(block.data));
            block.data = null;
        }

        private void ReleaseAll(bool finalRelease)
        {
            ++this.key;
            MessageHeap.BlockInfo block;
            if (finalRelease)
            {
                block = this.headBlock;
                this.headBlock = null;
                this.tailBlock = null;
            }
            else
            {
                this.ZeroMemoryBlock(this.headBlock, 0U, this.blockHeaderSize);
                block = this.headBlock.next;
                this.headBlock.next = null;
                this.headBlock.used = this.blockHeaderSize;
                this.tailBlock = this.headBlock;
                this.nextBlockId = 2U;
            }
            MessageHeap.BlockInfo next;
            for (; block != null; block = next)
            {
                next = block.next;
                switch (block.type)
                {
                    case BlockType.First:
                    case BlockType.Normal:
                    case BlockType.Large:
                        this.DestroyBlock(block);
                        break;
                }
            }
        }

        private unsafe MessageHeap.BlockInfo LookupBlockWorker(void* p)
        {
            for (MessageHeap.BlockInfo blockInfo = this.headBlock; blockInfo != null; blockInfo = blockInfo.next)
            {
                if (p >= blockInfo.data)
                {
                    void* voidPtr = (void*)((int)blockInfo.data + (int)blockInfo.used);
                    if (p < voidPtr)
                        return blockInfo;
                }
            }
            throw new ArgumentException("pointer is not on this heap");
        }

        private unsafe void ZeroMemoryBlock(MessageHeap.BlockInfo block, uint offset, uint size)
        {
            Debug2.Validate(offset + size <= block.size, typeof(ArgumentOutOfRangeException), "Invalid memory range for block");
            byte* numPtr1 = (byte*)((int)block.data + (int)offset);
            for (byte* numPtr2 = numPtr1 + (int)size; numPtr1 < numPtr2; ++numPtr1)
                *numPtr1 = 0;
        }

        protected override void Invariant() => Debug2.Validate(!this.IsDisposed, typeof(InvalidOperationException), "MessageHeap has already been disposed");

        internal struct Enumerator
        {
            private MessageHeap heap;
            private MessageHeap.BlockInfo current;
            private uint key;

            public Enumerator(MessageHeap heap)
            {
                this.heap = heap;
                this.current = null;
                this.key = heap.key;
            }

            public bool MoveNext()
            {
                if ((int)this.key != (int)this.heap.key)
                    throw new InvalidOperationException("heap modified while enumerator is in use");
                this.current = this.current != null ? this.current.next : this.heap.headBlock;
                return this.current != null;
            }

            public unsafe MessageHeap.Block Current
            {
                get
                {
                    if ((int)this.key != (int)this.heap.key)
                        throw new InvalidOperationException("heap modified while enumerator in use");
                    return new MessageHeap.Block(this.current.id, this.current.data, this.current.used);
                }
            }
        }

        internal struct Block
        {
            public uint id;
            public unsafe void* data;
            public uint size;

            public unsafe Block(uint id, void* data, uint size)
            {
                this.id = id;
                this.data = data;
                this.size = size;
            }
        }

        private enum BlockType
        {
            First,
            Normal,
            Large,
        }

        private class BlockInfo
        {
            public MessageHeap.BlockInfo next;
            public MessageHeap.BlockType type;
            public uint id;
            public unsafe void* data;
            public uint size;
            public uint used;
        }
    }
}
