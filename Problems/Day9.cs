using System.Text;

namespace advent_2024.problems;

public class Day9 : Day<long, long>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
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
        public int FileID;

        public Block(int fileID)
        {
            FileID = fileID;
        }

        public override string ToString()
        {
            return FileID == -1 ? "." : FileID.ToString();
        }
    }
    
    protected override long ExecuteTask2(string input)
    {
        var blockList = ParseInput2(input);
        // Console.WriteLine(String.Join("|", blockList));
        SwapBlocks(blockList);
        // Console.WriteLine(String.Join("|", blockList));

        long result = 0;

        for (int i = 0; i < blockList.Count; i++)
        {
            if (blockList[i].FileID != -1)
                result += blockList[i].FileID * i;
        }

        return result;
    }

    private void SwapBlocks(List<Block> blockList)
    {
        for (int i = blockList.Count - 1; i >= 0; i--)
        {
            var block = blockList[i];
            if (block.FileID != -1)
            {
                // Console.WriteLine(block.FileID);
                int fileEnd = i;
                int fileID = block.FileID;
                while (i > 0 && blockList[i-1].FileID == fileID)
                    i--;
                int fileStart = i;
                int fileSize = fileEnd - fileStart + 1;

                int freeSize = 0;
                int freeStart = 0;
                for (int j = 0; j < i; j++)
                {
                    if (blockList[j].FileID == -1)
                    {
                        if (freeSize == 0)
                            freeStart = j;
                        freeSize++;
                        if (freeSize >= fileSize)
                        {
                            for (int freeOverwrite = freeStart; freeOverwrite <= j; freeOverwrite++)
                                blockList[freeOverwrite].FileID = fileID;
                            for (int fileOverwrite = fileStart; fileOverwrite <= fileEnd; fileOverwrite++)
                                blockList[fileOverwrite].FileID = -1;
                            break;
                        }
                    }
                    else
                        freeSize = 0;
                }
                // Console.WriteLine(String.Join("|", blockList));
            }
        }
    }

    private List<Block> ParseInput2(string input)
    {
        List<Block> result = new List<Block>();
        int fileID = 0;
        bool isFreeSpace = false;
        for (int i = 0; i < input.Length; i++)
        {
            var size = int.Parse(input[i].ToString());
            int fileIDToConsider = isFreeSpace ? -1 : fileID;
            for (int j = 0; j < size; j++)
                result.Add(new Block(fileIDToConsider));
            if (!isFreeSpace)
                fileID++;
            isFreeSpace = !isFreeSpace;
        }

        return result;
    }
}