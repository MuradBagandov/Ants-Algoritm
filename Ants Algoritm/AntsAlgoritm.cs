using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Ants_Algoritm
{
    public struct Transition
    {
        public float Distane;
        public float Feromones;
        public int Number;

        public Transition(float distane, float feromones, int number)
        {
            Distane = distane;
            Feromones = feromones;
            Number = number;
        }
    }

    public class GraphVertex
    {
        public Vector2 Position;
        public int Number;
        public List<Transition> Transitions;

        public GraphVertex(Vector2 position)
        {
            Position = position;
            Transitions = new List<Transition>();
        }
    }


    public class AntsAlgoritm
    {
        private const float coeffDistanceEffec = 1.0f;
        private const float coeffFeromonesEffec = 1.0f;
        Random random = new Random();
        private List<GraphVertex> Vertex = new List<GraphVertex>();

        
        public AntsAlgoritm(List<Vector2> points)
        {
            int i=0, j=0;
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
                        _vertex.Transitions.Add(new Transition(distance, 0.2f, j));
                    }
                    j++;
                }
                Vertex.Add(_vertex);
            }
        }

        public void Generate(int iteration)
        {
            for (int iter = 1; iter < iteration; iter++)
            {
                List<(List<int> trajectory, float lenght)> trajectories = new List<(List<int>, float)>();
                for (int i = 0; i < Vertex.Count; i++)
                {
                    var s = Simulate(i);
                    trajectories.Add(s);
                }


                var optimalTrajectory = trajectories.OrderBy(i => i.lenght).First();
                for (int i = 0; i < optimalTrajectory.trajectory.Count - 1; i++)
                {
                    var vert = GetVertex(optimalTrajectory.trajectory[i]);
                    for (int j = 0; j < vert.Transitions.Count; j++)
                    {
                        if (vert.Transitions[j].Number == optimalTrajectory.trajectory[i + 1])
                        {
                            vert.Transitions[j] = new Transition(
                                vert.Transitions[j].Distane,
                                vert.Transitions[j].Feromones + 4 / optimalTrajectory.lenght,
                                vert.Transitions[j].Number);
                        }

                    }
                }
            }
        }


        public List<Vector2> ComputeTrajectory(int BeginVertexNumber)
        {
            if (BeginVertexNumber > Vertex.Count)
                throw new ArgumentException();

            var sim = Simulate(BeginVertexNumber);

            return sim.trajectory.Select(i => Vertex[i].Position).ToList();
        }

        public (List<int> trajectory, float lenght) Simulate(int BeginNumber)
        {
            List<int> trajectory = new List<int>();
            float length = 0;
            GraphVertex currentVertex = Vertex.Where(i => i.Number == BeginNumber).FirstOrDefault();
            
            trajectory.Add(BeginNumber);

            while (true)
            {
                List<(int number, double p)> probabilities = new List<(int, double)>();
                foreach (Transition t in currentVertex.Transitions)
                {
                    if (trajectory.Where(i => i == t.Number).Count() > 0) 
                        continue;
                    double p = Math.Pow(200.0/t.Distane, coeffDistanceEffec) * Math.Pow(t.Feromones, coeffFeromonesEffec);
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

                length += currentVertex.Transitions.Where(i => i.Number == nextVertext).First().Distane;
                currentVertex = Vertex.Where(i => i.Number == nextVertext).FirstOrDefault();
                
                trajectory.Add(nextVertext);
            }

            return (trajectory, length);

        }

        private int RouletteProbability(List<(int number, double p)> p)
        {
            double r = random.NextDouble();

            double sumProbability = 0;
            foreach ((int number, double p) item in p)
            {
                sumProbability += item.p;
                if (sumProbability >= r)
                    return item.number;
            }
            throw new ArgumentException();
        }

        private GraphVertex GetVertex(int number) => Vertex.Where(i => i.Number == number).FirstOrDefault();

    }
}
