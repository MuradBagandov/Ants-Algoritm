using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Ants_Algoritm
{
    public class GraphEdge
    {
        public float Distance;
        public float Feromones;
        public int Number;

        public GraphEdge(float distane, float feromones, int number)
        {
            Distance = distane;
            Feromones = feromones;
            Number = number;
        }
    }

    public class GraphVertex
    {
        public Vector2 Position;
        public int Number;
        public List<GraphEdge> Edges;

        public GraphVertex(Vector2 position)
        {
            Position = position;
            Edges = new List<GraphEdge>();
        }
    }


    public class AntsAlgoritm
    {
        private const float _DistanceEffectFactor = 1.0f;
        private const float _FeromonesEffectFactor = 1.0f;
        private const float _DefaultFeromones = 0.2f;
        private List<GraphVertex> _Vertices = new List<GraphVertex>();
        private Random _random = new Random();

        
        public AntsAlgoritm(List<Vector2> points)
        {
            int i=0, j;
            foreach(Vector2 item in points)
            {
                GraphVertex _vertex = new GraphVertex(item);
                _vertex.Number = i++;
                j = 0;
                foreach (Vector2 item2 in points)
                {
                    if (!item.Equals(item2))
                    {
                        float distance = Vector2.Distance(item2, item);
                        _vertex.Edges.Add(new GraphEdge(distance, _DefaultFeromones, j));
                    }
                    j++;
                }
                _Vertices.Add(_vertex);
            }
        }


        public void Reset()
        {
            foreach (GraphVertex vertex in _Vertices)
            {
                foreach (GraphEdge item in vertex.Edges)
                    item.Feromones = _DefaultFeromones;
            }
        }

        public void Generate(int iteration)
        {
            for (int iter = 0; iter < iteration; iter++)
            {
                List<(List<int> trajectory, float lenght)> trajectories = new List<(List<int>, float)>();
                for (int i = 0; i < _Vertices.Count; i++)
                {
                    var s = Simulate(i);
                    trajectories.Add(s);
                }

                (List<int> trajectory, float lenght) optimalTrajectory = trajectories.OrderBy(i => i.lenght).First();

                for (int i = 0; i < optimalTrajectory.trajectory.Count - 1; i++)
                {
                    var vert = GetVertex(optimalTrajectory.trajectory[i]);
                    for (int j = 0; j < vert.Edges.Count; j++)
                    {
                        if (vert.Edges[j].Number == optimalTrajectory.trajectory[i + 1])
                            vert.Edges[j].Feromones += +10 / optimalTrajectory.lenght;
                    }
                }
            }
        }


        public List<Vector2> ComputeTrajectory(int BeginVertexNumber)
        {
            if (BeginVertexNumber > _Vertices.Count)
                throw new ArgumentException();

            var sim = Simulate(BeginVertexNumber);
            return sim.trajectory.Select(i => _Vertices[i].Position).ToList();
        }

        public List<Vector2> ComputeOptimalTrajectory(int BeginVertexNumber, int Iteration)
        {
            if (BeginVertexNumber > _Vertices.Count)
                throw new ArgumentException();


            (List<int> trajectory, float lenght) optimalTrajectory = default;
            for (int i = 0; i < Iteration;i++)
            {
                var sim = Simulate(BeginVertexNumber);
                if (i == 0 || sim.lenght < optimalTrajectory.lenght)
                    optimalTrajectory = sim;
            }

            return optimalTrajectory.trajectory.Select(i => _Vertices[i].Position).ToList();
        }


        private (List<int> trajectory, float lenght) Simulate(int BeginNumber)
        {
            List<int> trajectory = new List<int>();
            float length = 0;
            GraphVertex currentVertex = _Vertices.Where(i => i.Number == BeginNumber).FirstOrDefault();
            
            trajectory.Add(BeginNumber);

            while (true)
            {
                List<(int number, double p)> probabilities = new List<(int, double)>();
                foreach (GraphEdge t in currentVertex.Edges)
                {
                    if (trajectory.Where(i => i == t.Number).Count() > 0) 
                        continue;
                    double p = Math.Pow(200.0/t.Distance, _DistanceEffectFactor) * Math.Pow(t.Feromones, _FeromonesEffectFactor);
                    probabilities.Add((t.Number, p));
                }
                if (probabilities.Count == 0)
                    break;

                List<(int number, double p)> probabilitiesVertexs = new List<(int, double)>();
                double overallProbability = probabilities.Select(i => i.p).Sum();
                foreach ((int number, double p) item in probabilities)
                {
                    probabilitiesVertexs.Add((item.number, item.p / overallProbability));
                }

                int nextVertext = RouletteProbability(probabilitiesVertexs);

                length += currentVertex.Edges.Where(i => i.Number == nextVertext).First().Distance;
                currentVertex = _Vertices.Where(i => i.Number == nextVertext).FirstOrDefault();
                
                trajectory.Add(nextVertext);
            }

            return (trajectory, length);

        }

        private int RouletteProbability(List<(int number, double p)> p)
        {
            double r = _random.NextDouble();

            double sumProbability = 0;
            foreach ((int number, double p) item in p)
            {
                sumProbability += item.p;
                if (sumProbability >= r)
                    return item.number;
            }
            throw new ArgumentException();
        }

        private GraphVertex GetVertex(int number) => _Vertices.Where(i => i.Number == number).FirstOrDefault();

    }
}
