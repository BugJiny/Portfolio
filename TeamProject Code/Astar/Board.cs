using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapInformation;


public class Board 
{
    public Block[,] blocks;   //맵타일

   
    public Board(int width, int height)
    {
        blocks = new Block[height, width];
        for (int i = 0; i < blocks.GetLength(0); i++)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                blocks[i, j] = new Block(i, j);
            }
        }
    }

    public void SetBlock(int x, int y, bool wall)
    {
        blocks[y, x].wall = wall;
    }

    public void CheckClear()
    {
        foreach (Block block in blocks)
        {
            block.Clear();
        }
    }

    public bool Exists(Block block)
    {
        return Exists(block.x, block.y);
    }

    public bool Exists(int x, int y)
    {
        foreach (Block block in blocks)
        {
            if (block.x == x && block.y == y)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 주변 블록 가져오기
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public List<Block> GetAroundBlocks(Block target)
    {
       
        List<Block> arounds = new List<Block>();

        //x==row y==column

        if(target.x % 2 ==0)
        {
           
            if (Exists(target.x - 1, target.y + 1) && !blocks[target.x - 1, target.y + 1].wall)
            {
                Block block = blocks[target.x - 1, target.y + 1];
                arounds.Add(block);
            }

            //오른쪽
            if (Exists(target.x, target.y+1) && !blocks[target.x, target.y +1].wall)
            {
                Block block = blocks[target.x, target.y+1];
                arounds.Add(block);
            }

            //우하단
            if (Exists(target.x + 1, target.y + 1) && !blocks[target.x + 1, target.y + 1].wall)
            {
                Block block = blocks[target.x + 1, target.y + 1];
                arounds.Add(block);
            }

            //좌하단
            if (Exists(target.x+1 , target.y ) && !blocks[target.x+1, target.y].wall)
            {
                Block block = blocks[target.x+1, target.y];
                arounds.Add(block);
            }

            //왼쪽
            if (Exists(target.x, target.y-1)&& !blocks[target.x, target.y-1].wall)
            {
                Block block = blocks[target.x, target.y-1];
                arounds.Add(block);
            }

            //좌상단
            if (Exists(target.x-1, target.y)&& !blocks[target.x-1, target.y].wall)
            {
                Block block = blocks[target.x-1, target.y];
                arounds.Add(block);
            }


        }
        else
        {
            if (Exists(target.x-1, target.y) && !blocks[target.x -1, target.y].wall)
            {
                Block block = blocks[target.x -1, target.y];
                arounds.Add(block);
            }

            //오른쪽
            if (Exists(target.x, target.y+1) && !blocks[target.x, target.y+1].wall)
            {
                Block block = blocks[target.x, target.y+1];
                arounds.Add(block);
            }

            //우하단
            if (Exists(target.x+1, target.y) && !blocks[target.x+1, target.y].wall)
            {
                Block block = blocks[target.x+1, target.y ];
                arounds.Add(block);
            }

            //좌하단
            if (Exists(target.x +1, target.y - 1) && !blocks[target.x + 1, target.y - 1].wall)
            {
                Block block = blocks[target.x +1, target.y - 1];
                arounds.Add(block);
            }

            //왼쪽
            if (Exists(target.x, target.y-1) && !blocks[target.x, target.y-1].wall)
            {
                Block block = blocks[target.x, target.y-1];
                arounds.Add(block);
            }

            //좌상단
            if (Exists(target.x -1, target.y - 1) && !blocks[target.x - 1, target.y - 1].wall)
            {
                Block block = blocks[target.x -1, target.y - 1];
                arounds.Add(block);
            }
        }


        // 대각선 블록인 경우 정방향블록이 벽이면 제외한다.
        //for (int i = arounds.Count - 1; i >= 0; i--)
        //{
        //    Block block = arounds[i];
        //    bool isDiagonalBlock = Mathf.Abs(block.x - target.x) == 1 && Mathf.Abs(block.y - target.y) == 1;
        //    if (isDiagonalBlock)
        //    {
        //        // 가로 블록 벽인지 확인
        //        Block blockX = arounds.Find(b => b.x == block.x && b.y == target.y);
        //        if (blockX.wall)
        //            arounds.Remove(block);

        //        // 세로 블록 벽인지 확인
        //        Block blockY = arounds.Find(b => b.x == target.x && b.y == block.y);
        //        if (blockY.wall)
        //            arounds.Remove(block);
        //    }
        //}

        // 벽 블록 제거
        arounds.RemoveAll(b => b.wall);

        return arounds;
    }
}
