using System.Text;

namespace advent_2024.problems;

public class Day9 : Day<long, long>
{
    protected override TaskState Task1State => TaskState.Test;
    protected override TaskState Task2State => TaskState.Test;
    
    protected override long ExecuteTask1(string input)
    {
        List<int> blocks = ParseInput1(input);
        //Console.WriteLine(String.Join(',', blocks));
        List<int> swappedBlocks = SwapBlocks(blocks);
        //Console.WriteLine(String.Join(',', swappedBlocks));
        long checkSum = ComputeChecksum(swappedBlocks);
        return checkSum;
    }

    private long ComputeChecksum(List<int> swappedBlocks)
    {
        long checksum = 0;
        for (int i = 0; i < swappedBlocks.Count; i++)
        {
            if (swappedBlocks[i] == -1)
                return checksum;
            
            checksum += swappedBlocks[i] * i;
        }
        
        return checksum;
    }

    private List<int> SwapBlocks(List<int> blocks)
    {
        int startIndex = 0;
        int endIndex = blocks.Count - 1;

        while (true)
        {
            while (blocks[endIndex] == -1)
                endIndex--;
            
            while (blocks[startIndex] != -1)
                startIndex++;

            if (endIndex <= startIndex)
                break;
         
            blocks[startIndex] = blocks[endIndex];
            blocks[endIndex] = -1;
        }

        return blocks;
    }

    private List<int> ParseInput1(string input)
    {
        List<int> blocks = new List<int>();
        int fileID = 0;
        bool isFreeSpace = false;
        foreach (var c in input)
        {
            var size = int.Parse(c.ToString());
            for (int i = 0; i < size; i++)
            {
                if (isFreeSpace)
                    blocks.Add(-1);
                else
                    blocks.Add(fileID);
            }

            if (!isFreeSpace)
                fileID++;
            
            isFreeSpace = !isFreeSpace;
        }

        return blocks;
    }

    private class Block
    {
        private int m_FileID;
        public int FileID => m_FileID;

        private int m_Size;
        public int Size => m_Size;

        private Block? m_PreviousBlock = null;
        public Block? PreviousBlock
        {
            get => m_PreviousBlock;
            set => m_PreviousBlock = value;
        }

        private Block? m_NextBlock = null;
        public Block? NextBlock
        {
            get => m_NextBlock;
            set => m_NextBlock = value;
        }

        public bool IsFree => m_FileID == -1;

        public Block(int fileID, int size)
        {
            m_FileID = fileID;
            m_Size = size;
        }

        public void UseSize(int diff)
        {
            m_Size -= diff;
        }

        public void AddSize(int diff)
        {
            m_Size += diff;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_Size; i++)
            {
                sb.Append(m_FileID == -1 ? "." : m_FileID);
                sb.Append('|');
            }
                

            return sb.ToString();
        }

        public int GetChecksumPart(ref int startIndex)
        {
            int result = 0;
            int maxIndex = startIndex + m_Size;
            for (; startIndex < maxIndex; startIndex++)
            {
                if (m_FileID >= 0)
                    result += startIndex * m_FileID;
            }                
            return result;
        }
    }

    private class BlockList : LinkedList<Block>
    {
        public BlockList() : base() { }
    }
    

    protected override long ExecuteTask2(string input)
    {
        var blockList = ParseInput2(input);
        // Console.WriteLine(String.Join("", blockList));
        SwapBlocks(blockList);
        // Console.WriteLine(String.Join("", blockList));
        
        long result = 0;
        int startIndex = 0;
        var block = blockList.First;
        while (block != null)
        {
            result += block.Value.GetChecksumPart(ref startIndex);
            block = block.Next;
        }

        return result;
    }

    private void SwapBlocks(BlockList blockList)
    {
        var itFile = blockList.Last;
        while (itFile != null)
        {
            if (!itFile.Value.IsFree)
            {
                // Console.WriteLine(itFile.Value.FileID);
                var fileToMove = itFile.Value;
                var itFree = blockList.First;
                while (itFree != null && itFree != itFile)
                {
                    if (itFree.Value.IsFree && itFree.Value.Size >= fileToMove.Size)
                    {
                        var orphan = itFile.Previous;
                        
                        // Remove the file from where it was... 
                        blockList.Remove(itFile);
                        
                        // Add it before the freeBlock
                        blockList.AddBefore(itFree, itFile);
                        
                        // Resize the free block
                        itFree.Value.UseSize(fileToMove.Size);
                        
                        // Remove it if it's empty
                        if (itFree.Value.Size == 0)
                            blockList.Remove(itFree);
                        
                        // Merge the free blocks around where the file was
                        if (orphan.Value.IsFree)
                            orphan.Value.AddSize(fileToMove.Size);
                        else if (orphan.Next != null && orphan.Next.Value.IsFree)
                            orphan.Next.Value.AddSize(fileToMove.Size);
                        else 
                            blockList.AddAfter(orphan, new Block(-1, fileToMove.Size));
                        
                        if (orphan.Value.IsFree && orphan.Next != null && orphan.Next.Value.IsFree)
                        {
                            orphan.Value.AddSize(orphan.Next.Value.Size);
                            blockList.Remove(orphan.Next);
                        }

                        itFile = orphan;
                        break;
                    }
                    itFree = itFree.Next;
                }
                // Console.WriteLine(String.Join("", blockList));
            }
            itFile = itFile.Previous;
        }
    }

    private BlockList ParseInput2(string input)
    {
        BlockList res = new BlockList();
        
        bool isFreeSpace = false;
        int fileID = 0;
        
        for (var index = 0; index < input.Length; index++)
        {
            var c = input[index];
            int size = int.Parse(c.ToString());

            Block nextBlock;
            if (!isFreeSpace)
            {
                nextBlock = new Block(fileID, size); 
                fileID++;
            }
            else
            {
                nextBlock = new Block(-1, size);
            }

            res.AddLast(nextBlock);
            isFreeSpace = !isFreeSpace;
        }

        return res;
    }
}