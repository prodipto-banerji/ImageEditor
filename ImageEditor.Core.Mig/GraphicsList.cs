using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using SkiaSharp;

namespace ImageEditor.Core
{
    /// <summary>
    /// List of graphic objects
    /// </summary>
    [Serializable]
    public class GraphicsList : IDisposable
    {
        private ArrayList graphicsList;
        private bool _isDirty;

        public bool Dirty
        {
            get
            {
                if (_isDirty == false)
                {
                    foreach (DrawObject o in graphicsList)
                    {
                        if (o.Dirty)
                        {
                            _isDirty = true;
                            break;
                        }
                    }
                }
                return _isDirty;
            }
            set
            {
                foreach (DrawObject o in graphicsList)
                    o.Dirty = false;
                _isDirty = false;
            }
        }

        public IEnumerable<DrawObject> Selection
        {
            get
            {
                foreach (DrawObject o in graphicsList)
                {
                    if (o.Selected)
                    {
                        yield return o;
                    }
                }
            }
        }

        private const string entryCount = "ObjectCount";
        private const string entryType = "ObjectType";

        public GraphicsList()
        {
            graphicsList = new ArrayList();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this.graphicsList != null)
                    {
                        for (int i = 0; i < this.graphicsList.Count; i++)
                        {
                            if (this.graphicsList[i] != null)
                            {
                                ((DrawObject)this.graphicsList[i]).Dispose();
                            }
                        }
                    }
                }
                this._disposed = true;
            }
        }

        ~GraphicsList()
        {
            this.Dispose(false);
        }

        public void LoadFromStream(SerializationInfo info, int orderNumber)
        {
            graphicsList = new ArrayList();
            int numberObjects = info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                              "{0}{1}",
                              entryCount, orderNumber));
            for (int i = 0; i < numberObjects; i++)
            {
                string typeName;
                typeName = info.GetString(
                    String.Format(CultureInfo.InvariantCulture,
                                  "{0}{1}",
                                  entryType, i));
                object drawObject;
                drawObject = Assembly.GetExecutingAssembly().CreateInstance(
                    typeName);
                ((DrawObject)drawObject).LoadFromStream(info, orderNumber, i);
                graphicsList.Add(drawObject);
            }
        }

        public void SaveToStream(SerializationInfo info, int orderNumber)
        {
            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                              "{0}{1}",
                              entryCount, orderNumber),
                graphicsList.Count);
            int i = 0;
            foreach (DrawObject o in graphicsList)
            {
                info.AddValue(
                    String.Format(CultureInfo.InvariantCulture,
                                  "{0}{1}",
                                  entryType, i),
                    o.GetType().FullName);
                o.SaveToStream(info, orderNumber, i);
                i++;
            }
        }

        /// <summary>
        /// Draw all the visible objects in the List
        /// </summary>
        /// <param name="canvas">SKCanvas to draw on</param>
        public void Draw(SKCanvas canvas)
        {
            int numberObjects = graphicsList.Count;
            for (int i = numberObjects - 1; i >= 0; i--)
            {
                DrawObject o = (DrawObject)graphicsList[i];
                o.Draw(canvas);
                if (o.Selected)
                    o.DrawTracker(canvas);
            }
        }

        public bool Clear()
        {
            bool result = (graphicsList.Count > 0);
            if (graphicsList.Count > 0)
            {
                for (int i = graphicsList.Count - 1; i >= 0; i--)
                {
                    if (graphicsList[i] != null)
                    {
                        ((DrawObject)graphicsList[i]).Dispose();
                    }
                    graphicsList.RemoveAt(i);
                }
            }
            if (result)
                _isDirty = false;
            return result;
        }

        public int Count
        {
            get { return graphicsList.Count; }
        }

        public DrawObject this[int index]
        {
            get
            {
                if (index < 0 || index >= graphicsList.Count)
                    return null;
                return (DrawObject)graphicsList[index];
            }
        }

        public int SelectionCount
        {
            get
            {
                int n = 0;
                foreach (DrawObject o in graphicsList)
                {
                    if (o.Selected)
                        n++;
                }
                return n;
            }
        }

        public DrawObject GetSelectedObject(int index)
        {
            int n = -1;
            foreach (DrawObject o in graphicsList)
            {
                if (o.Selected)
                {
                    n++;
                    if (n == index)
                        return o;
                }
            }
            return null;
        }

        public void Add(DrawObject obj)
        {
            graphicsList.Sort();
            foreach (DrawObject o in graphicsList)
                o.ZOrder++;
            graphicsList.Insert(0, obj);
        }
        public void AddAsInitialGraphic(DrawObject obj)
        {
            graphicsList.Add(obj);
        }
        public void Append(DrawObject obj)
        {
            graphicsList.Add(obj);
        }

        // Selection and ordering logic remains unchanged
        // Hit test and rectangle selection logic should be migrated to SkiaSharp types if needed
    }
}