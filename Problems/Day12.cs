using advent_2024.Framework.Math;

namespace advent_2024.problems;

public class Day12 : Day<int, long>
{
    protected override TaskState Task1State => TaskState.Final;
    protected override TaskState Task2State => TaskState.Final;
    
    private class FieldMap : Vector2Map<Field>
    {
        private List<Region> m_Regions;

        public List<Region> Regions => m_Regions;
        
        public FieldMap(Field[,] fields) : base(fields.GetLength(0), fields.GetLength(1))
        {
            m_Map = fields;
        }

        public void BuildRegions()
        {
            m_Regions = new List<Region>();
            
            List<Field> allFields = new List<Field>();
            foreach (var field in m_Map)
                allFields.Add(field);

            while (allFields.Count > 0)
            {
                Region region = new Region(allFields[0].Type);
                var queue = new Queue<Field>();
                queue.Enqueue(allFields[0]);
                while (queue.Count > 0)
                {
                    var field = queue.Dequeue();
                    allFields.Remove(field);
                    region.AddField(field);
                    
                    foreach (var direction in Vector2DirectionExtensions.NonDiagonals)
                    {
                        var neighbour = GetNeighbour(field.Position, direction);
                        if (neighbour == null)
                            continue;

                        if (!allFields.Contains(neighbour) || queue.Contains(neighbour))
                            continue;

                        if (neighbour.Type == field.Type)
                        {
                            queue.Enqueue(neighbour);
                        }
                    }
                }
                region.Finalise(this);
                m_Regions.Add(region);
            }
        }
    }

    private class Field
    {
        private string m_Type;
        public string Type => m_Type;

        private Vector2Int m_Position;
        public Vector2Int Position => m_Position;

        public Field(char type, Vector2Int position)
        {
            m_Type = type.ToString();
            m_Position = position;
        }

        public override string ToString() => m_Type;
    }

    private class Region
    {
        private string m_Type;
        public string Type => m_Type;

        private List<Field> m_Fields;

        private int m_Perimeter;
        public int Perimeter => m_Perimeter;

        public int Price => Perimeter * Area;
        public long Price2 => Corners * Area;

        private int m_Sides;
        public int Sides => m_Sides;
        
        private int m_Corners;
        public int Corners => m_Corners;
        
        public Region(string type)
        {
            m_Type = type;
            m_Fields = new List<Field>();
        }

        public void AddField(Field f)
        {
            m_Fields.Add(f);
        }

        public void Finalise(FieldMap map)
        {
            foreach (var field in m_Fields)
            {
                m_Perimeter += PerimeterContribution(map, field);
            }

            ComputeSides(map);
            ComputeSidesFromCorners(map);
        }

        private int PerimeterContribution(FieldMap map, Field f)
        {
            int perimeterContribution = 0;
            foreach (var neighbourDir in Vector2DirectionExtensions.NonDiagonals)
            {
                var neighbour = map.GetNeighbour(f.Position, neighbourDir);
                if (neighbour == null || !m_Fields.Contains(neighbour))
                    perimeterContribution++;
            }

            return perimeterContribution;
        }

        private void ComputeSides(FieldMap map)
        {
            int sideCount = 0;
            Dictionary<Field, List<Vector2Direction>> contributions = new Dictionary<Field, List<Vector2Direction>>();
            foreach (var f in m_Fields)
            {
                contributions.Add(f, new List<Vector2Direction>());
                foreach (var neighbourDir in Vector2DirectionExtensions.NonDiagonals)
                {
                    var neighbour = map.GetNeighbour(f.Position, neighbourDir);
                    if (neighbour == null || !m_Fields.Contains(neighbour))
                    {
                        contributions[f].Add(neighbourDir);
                        
                        bool isValid = true;
                        foreach (var neighbourDir2 in Vector2DirectionExtensions.NonDiagonals)
                        {
                            var neighbour2 = map.GetNeighbour(f.Position, neighbourDir2);
                            if (neighbour2 == null)
                                continue;
                            
                            if (contributions.TryGetValue(neighbour2, out var alreadyContributed) && alreadyContributed.Contains(neighbourDir))
                            {
                                isValid = false;
                                break;
                            }
                        }

                        if (isValid)
                            sideCount++;
                    }
                }
            }

            m_Sides = sideCount;
        }

        private bool IsNeighbouringFieldInRegion(FieldMap map, Field f, Vector2Direction dir)
        {
            var neighbour = map.GetNeighbour(f.Position, dir);
            return neighbour != null && m_Fields.Contains(neighbour);
        }
        
        private void ComputeSidesFromCorners(FieldMap map)
        {
            int cornerCount = 0;

            foreach (var f in m_Fields)
            {
                int cornerCode = 0;
                if (IsNeighbouringFieldInRegion(map, f, Vector2Direction.W)) cornerCode += 1;
                if (IsNeighbouringFieldInRegion(map, f, Vector2Direction.E)) cornerCode += 2;
                if (IsNeighbouringFieldInRegion(map, f, Vector2Direction.S)) cornerCode += 4;
                if (IsNeighbouringFieldInRegion(map, f, Vector2Direction.N)) cornerCode += 8;
                
                switch (cornerCode)
                {
                    case 0:
                        cornerCount += 4;
                        break;
                    case 1:
                        cornerCount += 2;
                        break;
                    case 2:
                        cornerCount += 2;
                        break;
                    case 3:
                        break;
                    case 4:
                        cornerCount += 2;
                        break;
                    case 5:
                        cornerCount += 1;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.SW)) cornerCount++;
                        break;
                    case 6:
                        cornerCount += 1;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.SE)) cornerCount++;
                        break;
                    case 7:
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.SW)) cornerCount++;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.SE)) cornerCount++;
                        break;
                    case 8:
                        cornerCount += 2;
                        break;
                    case 9:
                        cornerCount += 1;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.NW)) cornerCount++;
                        break;
                    case 10:
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.NE)) cornerCount++;
                        cornerCount += 1;
                        break;
                    case 11:
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.NE)) cornerCount++;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.NW)) cornerCount++;
                        break;
                    case 12:
                        break;
                    case 13:
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.NW)) cornerCount++;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.SW)) cornerCount++;
                        break;
                    case 14:
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.NE)) cornerCount++;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.SE)) cornerCount++;
                        break;
                    case 15:
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.NE)) cornerCount++;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.NW)) cornerCount++;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.SE)) cornerCount++;
                        if (!IsNeighbouringFieldInRegion(map, f, Vector2Direction.SW)) cornerCount++;
                        break;
                }
            }

            m_Corners = cornerCount;
        }

        public int Area => m_Fields.Count;

        public override string ToString() => $"{m_Type}(A={Area}|P={Perimeter}|S={Sides}|C={Corners}$={Price}|$$={Price2})";
        public string GetFirstField()
        {
            return m_Fields[0].Position.ToString();
        }
    }
    
    protected override int ExecuteTask1(string input)
    {
        FieldMap fieldMap = ParseInput(input);
        fieldMap.BuildRegions();
        int price = 0;
        foreach (var region in fieldMap.Regions)
            price += region.Price;

        return price;
    }
    
    private FieldMap ParseInput(string input)
    {
        var lines = input.Split('\n');
        var width = lines[0].Length;
        var height = lines.Length;

        Field[,] fields = new Field[width, height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                fields[x, y] = new Field(lines[y][x], new Vector2Int(x, y));
            }
        }

        return new FieldMap(fields);
    }

    protected override long ExecuteTask2(string input)
    {
        FieldMap fieldMap = ParseInput(input);
        fieldMap.BuildRegions();
        long price = 0;
        int totalArea = 0;
        foreach (var region in fieldMap.Regions)
        {
            price += region.Price2;
            totalArea += region.Area;
        }

        return price;
    }
}

