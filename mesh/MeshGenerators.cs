﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace g3
{
    abstract public class MeshGenerator
    {
        public VectorArray3d vertices;
        public VectorArray2f uv;
        public VectorArray3f normals;
        public VectorArray3i triangles;

        public bool WantUVs = true;
        public bool WantNormals = true;
        public bool Clockwise = false;



        abstract public void Generate();


        public void MakeMesh(SimpleMesh m)
        {
            m.AppendVertices(vertices, (WantNormals) ? normals : null, null, (WantUVs) ? uv : null);
            m.AppendTriangles(triangles);
        }
        public void MakeMesh(DMesh3 m)
        {
            int nV = vertices.Count;
            for (int i = 0; i < nV; ++i) {
				NewVertexInfo ni = new NewVertexInfo() { v = vertices[i] };
				if ( WantNormals ) {
					ni.bHaveN = true; 
					ni.n = normals[i];
				}
				if ( WantUVs ) {
					ni.bHaveUV = true;
					ni.uv = uv[i];
				}
                int vID = m.AppendVertex(ni);
                Util.gDevAssert(vID == i);
            }
            int nT = triangles.Count;
            for (int i = 0; i < nT; ++i)
                m.AppendTriangle(triangles[i]);
        }




        public struct CircularSection
        {
            public float Radius;
            public float SectionY;
            public CircularSection(float r, float y)
            {
                Radius = r;
                SectionY = y;
            }
        }


        protected void duplicate_vertex_span(int nStart, int nCount)
        {
            for (int i = 0; i < nCount; ++i) {
                vertices[(nStart + nCount) + i] = vertices[nStart + i];
                normals[(nStart + nCount) + i] = normals[nStart + i];
                uv[(nStart + nCount) + i] = uv[nStart + i];
            }
        }


        protected void append_disc(int Slices, int nCenterV, int nRingStart, bool bClosed, bool bCycle, ref int tri_counter)
        {
            int nLast = nRingStart + Slices;
            for (int k = nRingStart; k < nLast-1; ++k)
                triangles.Set(tri_counter++, k, nCenterV, k + 1, bCycle);
            if (bClosed)      // close disc if we went all the way
                triangles.Set(tri_counter++, nLast-1, nCenterV, nRingStart, bCycle);
        }

        // assumes order would be [v0,v1,v2,v3], ccw
        protected void append_rectangle(int v0, int v1, int v2, int v3, bool bCycle, ref int tri_counter)
        {
            triangles.Set(tri_counter++, v0, v1, v2, bCycle);
            triangles.Set(tri_counter++, v0, v2, v3, bCycle);
        }

        protected Vector3f estimate_normal(int v0, int v1, int v2)
        {
            Vector3d a = vertices[v0];
            Vector3d b = vertices[v1];
            Vector3d c = vertices[v2];
            Vector3d e1 = (b - a).Normalized;
            Vector3d e2 = (c - a).Normalized;
            return new Vector3f(e1.Cross(e2));
        }
    }








}
