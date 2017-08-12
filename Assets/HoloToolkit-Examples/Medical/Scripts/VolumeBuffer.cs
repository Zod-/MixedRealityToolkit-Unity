// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Represents a 3D array of data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VolumeBuffer<T>
    {
        public T[] DataArray;
        public readonly Int3 Size;

        public bool IsEdge(Int3 pos)
        {
            return (((pos.x == 0) || ((pos.x + 1) == Size.x)) ||
                    ((pos.y == 0) || ((pos.y + 1) == Size.y)) ||
                    ((pos.z == 0) || ((pos.z + 1) == Size.z)));
        }

        public bool IsValid(Int3 pos)
        {
            return (((pos.x >= 0) && (pos.x < Size.x)) &&
                    ((pos.y >= 0) && (pos.y < Size.y)) &&
                    ((pos.z >= 0) && (pos.z < Size.z)));
        }

        public VolumeBuffer(Int3 size)
        {
            Size = size;
            DataArray = new T[Size.sqrMagnitude];
        }

        public VolumeBuffer(Int3 size, T[] ar)
        {
            Size = size;
            DataArray = ar;
        }

        public T this[Int3 pos]
        {
            get { return GetVoxel(pos); }
            set { SetVoxel(pos, value); }
        }

        public T GetVoxel(Int3 pos)
        {
            return DataArray[MathExtensions.CubicToLinearIndex(pos, Size)];
        }

        public void SetVoxel(Int3 pos, T val)
        {
            DataArray[MathExtensions.CubicToLinearIndex(pos, Size)] = val;
        }

        public void ClearEdges(T clearVal)
        {
            //TODO: there's no need to test - could just explicitly walk the edges
            for (var i = 0; i < Size.sqrMagnitude; ++i)
            {
                var ndx = MathExtensions.LinearToCubicIndex(i, Size);
                if (IsEdge(ndx))
                {
                    this[ndx] = clearVal;
                }
            }
        }
    }
}