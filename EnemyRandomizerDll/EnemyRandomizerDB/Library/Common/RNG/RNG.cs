﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Meisui.Random;

namespace EnemyRandomizerMod
{
    /// <summary>
    /// This class may be used to produce a sequence of pseudorandom numbers using the mersene twister algorithm
    /// </summary>
    [Serializable]
    public class RNG
    {
        //a singleton type that may be used for convenience and when deterministic behaviour isn't needed
        public static RNG prng;

        //a singleton type that may be used for convenience and when deterministic behaviour isn't needed
        public static RNG pRNG
        {
            get
            {
                if (prng == null)
                {
                    prng = new RNG();
                    prng.Reset();
                }

                return prng;
            }
        }

        [SerializeField]
        MersenneTwister _mtRNG;

        MersenneTwister mtRNG {
            get {
                if( _mtRNG == null )
                    Reset();
                return _mtRNG;
            }
            set {
                _mtRNG = value;
            }
        }

        [SerializeField]
        int _seed;

        public int Seed {
            get {
                return _seed;
            }
            set {
                Reset(value);
            }
        }

        public RNG()
        {
        }

        public RNG( int s )
        {
            Reset(s);
        }

        //Reset the RNG internal engine with a new random seed
        public void Reset()
        {
            int newseed = UnityEngine.Random.Range( Int32.MinValue, Int32.MaxValue );
            Reset( newseed );
        }

        //Reset the RNG internal engine with a given seed
        public void Reset( int s )
        {
            _seed = s;
            UInt32 i32seed = (UInt32)( s );
            mtRNG = new MersenneTwister( i32seed );
        }

        public uint Rand()
        {
            return mtRNG.genrand_Int32();
        }

        public uint Rand( uint r )
        {
            return Rand() % r;
        }

        //Generates a [0-N] int
        public int Rand( int value )
        {
            int random_value;
            if( value == 0 )
                return 0;

            random_value = (int)( ( (float)mtRNG.genrand_Int32() / (float)0xFFFFFFFF ) * ( value ) );
            return random_value;
        }

        public int Randi()
        {
            return (int)Randl();
        }

        public long Randl()
        {
            return mtRNG.genrand_Int31();
        }

        ///Generates a [0-1] float
        public float Randf()
        {
            return (float)( mtRNG.genrand_real1() );
        }

        ///Generates a [0-N] float
        public float Rand( float value )
        {
            return Randf() * value;
        }

        public double Randd()
        {
            return mtRNG.genrand_real1();
        }

        ///Generates a [0-N] double
        public double Rand( double a )
        {
            double n = Randd() * a;
            return n;
        }

        public uint Rand( uint a, uint b )
        {
            if(a == b) return a;
            if( a == 0 ) return Rand( b );
            if( a > b ) Mathnv.Swap( ref a, ref b );
            uint n = 2 * b;
            uint m = a + b;
            uint c = n % m;
            return a + Rand() % c;
        }

        public int Rand( int a, int b )
        {
            if(a == b) return a;
            if( a > b ) Mathnv.Swap( ref a, ref b );

            int c = b - a;
            return a + Randi() % c;
        }

        public float Rand( float a, float b )
        {
            if(a == b) return a;
            if( a > b ) Mathnv.Swap( ref a, ref b );

            float c = b - a;
            return a + Randf() * c;
        }

        public double Rand( double a, double b )
        {
            if(a == b) return a;
            if( a > b ) Mathnv.Swap( ref a, ref b );

            double c = b - a;
            return a + Randd() * c;
        }

        // rolling min or max will be rare, but rolling exactly between the two will be common
        public double GaussianRandom( double min, double max )
        {
            if(min == max) return min;
            if( min > max ) Mathnv.Swap<double>( ref min, ref max );
            min /= 3;
            max /= 3;
            return Rand( min, max ) + Rand( min, max ) + Rand( min, max ); ;
        }

        //random point from [ [0f,0f] , [a.x,a.y] ]
        public Vector2 Rand( Vector2 a )
        {
            float x = Randf() * a.x;
            float y = Randf() * a.y;
            return new Vector2( x, y );
        }

        //random point in an area
        public Vector2 Rand( Vector2 a, Vector2 b )
        {
            if( a.x > b.x ) Mathnv.Swap( ref a.x, ref b.x );
            if( a.y > b.y ) Mathnv.Swap( ref a.y, ref b.y );

            float cx = b.x - a.x;
            float cy = b.y - a.y;
            return new Vector2( a.x + Randf() * cx, a.y + Randf() * cy );
        }

        //random point in an area
        public Vector2 Rand( Rect a )
        {
            float cx = a.width;
            float cy = a.height;
            return new Vector2( a.x + Randf() * cx, a.y + Randf() * cy );
        }

        // random vector
        //from [ [min.x,min.y] , [max.x,max.y] ]
        public Vector3 RandVec3( float min, float max )
        {
            Vector3 vec = new Vector3(Rand(min, max), Rand(min, max), Rand(min, max)).normalized;
            return vec;
        }

        // random normalized vector
        //from [ [min.x,min.y] , [max.x,max.y] ]
        public Vector3 RandVec3Normalized( float min, float max )
        {
            if( min > max ) Mathnv.Swap( ref min, ref max );
            Vector3 vec = new Vector3(Rand(min, max), Rand(min, max), Rand(min, max)).normalized;
            vec.Normalize();
            return vec;
        }

        //Returns true if generated value [0-value] is less than limit
        public bool RandomLowerThanLimit( int limit, int value )
        {
            if( value == 0 )
                return false;
            return ( Rand( value ) < limit );
        }

        //50/50 RNG
        public bool CoinToss()
        {
            return Rand( 2 ) != 0;
        }

        //uses a given distribution and a range to weight the outcomes
        //NOTE: Assumes the X range of the curve to be [0,1]
        public int WeightedRand(AnimationCurve distribution, int min, int max)
        {
            float range = max - min;

            //integrate the curve
            float maxWeight = 0f;
            for(int i = 0; i < range; ++i)
            {
                float t = i;
                maxWeight += distribution.Evaluate(t / range);
            }

            float rngValue = Rand(maxWeight);

            //find the outcome
            float sum = 0f;
            for(int i = 0; i < range; ++i)
            {
                float t = i;
                sum += distribution.Evaluate(t / range);
                if(sum > rngValue)
                    return min + i;
            }
            return max;
        }

        //uses a given distribution and a range to weight the outcomes
        //returns the selected random index
        public int WeightedRand(List<float> distribution)
        {
            //integrate the curve
            float maxWeight = 0f;
            for(int i = 0; i < distribution.Count; ++i)
            {
                maxWeight += distribution[i];
            }

            float rngValue = Rand(0f, maxWeight);

            //find the outcome
            float sum = 0f;
            for(int i = 0; i < distribution.Count; ++i)
            {
                sum += distribution[i];
                if(sum > rngValue)
                    return i;
            }
            return distribution.Count - 1;
        }

        public T RandomElement<T>(List<T> elements)
        {
            return elements[Rand(0, elements.Count)];
        }

        public void RandomShuffle<T>(List<T> elements)
        {
            int n = elements.Count();
            for(int i = n-1; i > 0; --i)
            {
                Mathnv.Swap(elements, i, Rand(i + 1));
            }
        }

        public void Shuffle2D<T>(ref T[][] data)
        {
            int n = data.Length;
            while(n > 1)
            {
                n--;
                int k = Rand(n + 1);
                int innerLength = data[k].Length;
                T[] value = new T[innerLength];
                for(int i = 0; i < innerLength; ++i)
                    value[i] = data[k][i];

                for(int i = 0; i < innerLength; ++i)
                    data[k][i] = data[n][i];

                for(int i = 0; i < innerLength; ++i)
                    data[n][i] = value[i];
            }
        }

        /// <summary>
        /// Gets a random angle in radians from 0 to 2PI
        /// </summary>
        public double RandomAngled()
        {
            double r = Rand(0.0, 360.0) * Math.PI / 180.0;
            return r;
        }

        public double RandomAngled(float maxTheta)
        {
            double r = Rand(0.0, maxTheta) * Math.PI / 180.0;
            return r;
        }

        public double RandomAngled(float minTheta, float maxTheta)
        {
            double r = Rand(minTheta, maxTheta) * Math.PI / 180.0;
            return r;
        }

        /// <summary>
        /// Gets a random angle in radians from 0 to 2PI
        /// </summary>
        public float RandomAngle()
        {
            return (float)RandomAngled();
        }

        public float RandomAngle(float maxTheta)
        {
            return (float)RandomAngled(maxTheta);
        }

        public double RandomAngle(float minTheta, float maxTheta)
        {
            return (float)RandomAngled(minTheta, maxTheta);
        }

        public Vector2 RandomPointOnCircle(float r)
        {
            return RandomPointOnCircle(Vector2.one * r);
        }

        public Vector2 RandomPointOnCircle(Vector2 size)
        {
            float theta = RandomAngle();
            return new Vector2(size.x * Mathf.Cos(theta), size.y * Mathf.Sin(theta));
        }
    }

    public static class RNGExtensions
    {
        public static T GetRandomElement<T>(this RNG rng, ArrayGrid<T> grid)
        {
            Vector2Int p = Vector2Int.FloorToInt(rng.Rand(grid.SizeRect));
            return grid[p];
        }

        public static Vector2Int GetRandomPosition(this RNG rng, Vector2Int area)
        {
            Vector2Int p = Vector2Int.FloorToInt(rng.Rand(area));
            return p;
        }

        public static T GetRandomNonEmptyElement<T>(this RNG rng, ArrayGrid<T> grid)
        {
            List<T> elements = grid.GetNonEmptyElements().ToList();

            if (elements.Count <= 0)
                return default(T);

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static Vector2Int? GetRandomNonEmptyPosition<T>(this RNG rng, ArrayGrid<T> grid)
        {
            List<Vector2Int> elements = grid.GetNonEmptyPositions();

            if (elements.Count <= 0)
                return null;

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        //Warning: Expensive call
        public static T GetRandomEmptyElement<T>(this RNG rng, ArrayGrid<T> grid)
        {
            List<T> elements = grid.GetEmptyElements().ToList();

            if (elements.Count <= 0)
                return default(T);

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        //Warning: Expensive call
        public static Vector2Int? GetRandomEmptyPosition<T>(this RNG rng, ArrayGrid<T> grid)
        {
            List<Vector2Int> elements = grid.GetEmptyPositions();

            if (elements.Count <= 0)
                return null;

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static T GetRandomMatchingElement<T>(this RNG rng, ArrayGrid<T> grid, T dataToMatch)
        {
            List<T> elements = grid.GetMatchingElements(dataToMatch).ToList();

            if (elements.Count <= 0)
                return default(T);

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static Vector2Int? GetRandomPositionOfType<T>(this RNG rng, ArrayGrid<T> grid, T dataToMatch)
        {
            List<Vector2Int> elements = grid.GetMatchingPositions(dataToMatch);

            if (elements.Count <= 0)
                return null;

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static T GetRandomElementInArea<T>(this RNG rng, ArrayGrid<T> grid, Rect area)
        {
            Mathnv.Clamp(ref area, grid.Area);

            Vector2Int p = Vector2Int.FloorToInt(rng.Rand(area));

            return grid[p];
        }

        public static Vector2Int? GetRandomPositionInArea<T>(this RNG rng, ArrayGrid<T> grid, Rect area)
        {
            Mathnv.Clamp(ref area, grid.Area);

            Vector2Int p = Vector2Int.FloorToInt(rng.Rand(area));

            return p;
        }

        public static T GetRandomMatchingElementInArea<T>(this RNG rng, ArrayGrid<T> grid, Rect area, T dataToMatch)
        {
            Mathnv.Clamp(ref area, grid.SizeRect);

            List<T> elements = grid.GetMatchingElementsInArea(area, dataToMatch);

            if (elements.Count <= 0)
                return default(T);

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static Vector2Int? GetRandomPositionInAreaOfType<T>(this RNG rng, ArrayGrid<T> grid, Rect area, T dataToMatch)
        {
            Mathnv.Clamp(ref area, grid.SizeRect);

            List<Vector2Int> elements = grid.GetMatchingPositionsInArea(area, dataToMatch);

            if (elements.Count <= 0)
                return null;

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static T GetRandomElementInAreaInMask<T>(this RNG rng, ArrayGrid<T> grid, Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, grid.SizeRect);

            List<T> elements = grid.GetElementsInAreaInMask(area, mask);

            if (elements.Count <= 0)
                return default(T);

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static Vector2Int? GetRandomPositionInAreaInMask<T>(this RNG rng, ArrayGrid<T> grid, Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, grid.SizeRect);

            List<Vector2Int> elements = grid.GetPositionsInAreaInMask(area, mask);

            if (elements.Count <= 0)
                return null;

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static T GetRandomElementInAreaNotInMask<T>(this RNG rng, ArrayGrid<T> grid, Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, grid.SizeRect);

            List<T> elements = grid.GetElementsInAreaNotInMask(area, mask);

            if (elements.Count <= 0)
                return default(T);

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static Vector2Int? GetRandomPositionInAreaNotInMask<T>(this RNG rng, ArrayGrid<T> grid, Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, grid.SizeRect);

            List<Vector2Int> elements = grid.GetPositionsInAreaNotInMask(area, mask);

            if (elements.Count <= 0)
                return null;

            int i = rng.Rand(elements.Count);

            return elements[i];
        }

        public static Rect GetRandomArea<T>(this RNG rng, ArrayGrid<T> grid, Vector2Int size)
        {
            Rect area = new Rect(Vector2Int.zero, grid.Size);

            if (size.x > grid.w)
                return area;
            if (size.y > grid.h)
                return area;

            area = new Rect(Vector2Int.zero, size);

            Vector2Int p = Vector2Int.FloorToInt(rng.Rand(grid.Area));

            if (p.x + size.x > grid.w)
            {
                p.x += grid.w - (p.x + size.x);
            }

            if (p.y + size.y > grid.h)
            {
                p.y += grid.h - (p.y + size.y);
            }

            area.position = p;

            return area;
        }

        public static bool GetMatchingPositionOfRandomArea<T>(this RNG rng, ArrayGrid<T> grid, T dataToMatch, Vector2Int desiredSize, ref Vector2Int position)
        {
            List<Vector2Int> areas = grid.GetMatchingPositionsOfAreas(dataToMatch, desiredSize);
            if (areas.Count <= 0)
                return false;

            position = areas.GetRandomElementFromList(rng);

            return true;
        }


        public static Vector2 GetRandomValue(this RNG rng, Rect r)
        {
            return new Vector2(r.GetXRange().RandomValuef(rng), r.GetYRange().RandomValuef(rng));
        }

        public static Vector2Int GetRandomValueInt(this RNG rng, Rect r)
        {
            return new Vector2Int(r.GetXRange().RandomValuei(rng), r.GetYRange().RandomValuei(rng));
        }

        //public static Vector2 GetRandomValue(this RNG rng, Rect r, RNG rng)
        //{
        //    return new Vector2(r.GetXRange().RandomValuef(rng), r.GetYRange().RandomValuef(rng));
        //}

        //public static Vector2Int GetRandomValueInt(this RNG rng, Rect r, RNG rng)
        //{
        //    return new Vector2Int(r.GetXRange().RandomValuei(rng), r.GetYRange().RandomValuei(rng));
        //}
    }
}