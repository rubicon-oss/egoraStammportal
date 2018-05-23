/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Xml.Serialization;

namespace Egora.Stammportal.HttpReverseProxy.Mapping
{
  [XmlRoot(Namespace = "http://www.egora.at/Stammportal/PathMap/1.0")]
  public class PathMap
  {
    public static PathMap CreateFromFile(string fileName)
    {
      XmlSerializer serializer = new XmlSerializer(typeof (PathMap));
      PathMap mapping;
      using (System.IO.StreamReader reader = new System.IO.StreamReader(fileName))
      {
        mapping = (PathMap) serializer.Deserialize(reader);
      }
      return mapping;
    }

    private Directory[] _directories;

    [XmlArray]
    [XmlArrayItem(Type = typeof (Directory))]
    [XmlArrayItem(Type = typeof (ApplicationDirectory))]
    public Directory[] Directories
    {
      get { return _directories; }
      set
      {
        if (!Directory.HasUniqueNames(value))
          throw new ApplicationException("Directories must have unique names.");

        _directories = value;
      }
    }

    public ApplicationDirectory GetApplication(string path)
    {
      Directory dir = GetDirectory(path);

      if (dir != null)
        return dir.Application;

      return null;
    }

    public Directory GetDirectory(string path)
    {
      Directory defaultDir = null;
      foreach (Directory d in _directories)
      {
        Directory dir = d.GetDirectory(path);
        if (dir != null)
          return dir;
        if (d.Name == String.Empty)
          defaultDir = d;
      }
      return defaultDir;
    }
  }
}