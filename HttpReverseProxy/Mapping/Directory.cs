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
  public class Directory
  {
    private string _name;
    private string _slashname;
    private string _slashnameslash;

    [XmlAttribute]
    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        _slashname = "/" + _name;
        _slashnameslash = "/" + _name + "/";
      }
    }

    private  bool _ignoreCase;

    [XmlAttribute]
    public bool IgnoreCase
    {
      get { return _ignoreCase || ((Parent == null) ? _ignoreCase : Parent.IgnoreCase); } 
      set { _ignoreCase = value; }
    }

    private string _targetPath;

    [XmlAttribute]
    public string TargetPath
    {
      get
      {
        if (_targetPath == null)
          TargetPath = Name;
        return _targetPath;
      }
      set
      {
        _targetPath = value;
      }
    }

    private string _fullTargetPath = null;

    [XmlIgnore]
    public virtual string FullTargetPath
    {
      get
      {
        if (_fullTargetPath == null)
        {
          if (_parent == null)
          {
            _fullTargetPath = "/" + TargetPath;
          }
          else
          {
            _fullTargetPath = _parent.FullTargetPath + "/" + TargetPath;
          }
        }

        return _fullTargetPath;
      }
    }

    private string _fullPath = null;

    [XmlIgnore]
    public virtual string FullPath
    {
      get
      {
        if (_fullPath == null)
        {
          if (_parent == null)
          {
            _fullPath = _slashname;
          }
          else
          {
            _fullPath = _parent.FullPath + _slashname;
          }
        }

        return _fullPath;
      }
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
        if (!HasUniqueNames(value))
          throw new ApplicationException("Directories must have unique names.");

        _directories = value;
        foreach (Directory subDir in _directories)
          subDir.Parent = this;
      }
    }

    private Directory _parent;

    [XmlIgnore]
    public virtual Directory Parent
    {
      get { return _parent; }
      private set
      {
        _parent = value;
        _fullTargetPath = null;
        _fullPath = null;
      }
    }

    [XmlIgnore]
    public virtual ApplicationDirectory Application
    {
      get
      {
        if (_parent != null)
          return _parent.Application;

        return null;
      }
    }

    [XmlIgnore]
    protected StringComparison Comparison
    {
      get { return IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture; }
    }

    private bool AreEqual(string path, string name)
    {
      return String.Compare(path, name, Comparison) == 0;
    }

    public virtual Directory GetDirectory(string path)
    {
      if (AreEqual(path,_slashname) || AreEqual(path, _slashnameslash))
        return this;

      if (!path.StartsWith(_slashnameslash, Comparison))
        return null;

      string subpath = path.Substring(_name.Length + 1);
      Directory defaultDir = null;
      if (Directories != null)
      {
        foreach (Directory subDir in Directories)
        {
          Directory dir = subDir.GetDirectory(subpath);
          if (dir != null)
            return dir;
          if (subDir.Name == String.Empty)
            defaultDir = subDir;
        }
        if (defaultDir != null)
          return defaultDir;
      }

      return this;
    }

    public virtual string GetFullTargetPath(string fullPath)
    {
      if (!fullPath.StartsWith(FullPath, Comparison))
        throw new ApplicationException("Wrong path.");

      string fullPathSub = fullPath.Substring(FullPath.Length);
      if (_parent == null && !fullPathSub.StartsWith ("/"))
        fullPathSub = "/" + fullPathSub;
      return FullTargetPath + fullPathSub;
    }

    //public virtual string GetTargetPath (string path)
    //{
    //  if (path == _slashname)
    //    return _slashtargetPath;

    //  if (path == _slashnameslash)
    //    return _slashtargetPathslash;

    //  if (!path.StartsWith (_slashnameslash))
    //    throw new ApplicationException ("Wrong path.");

    //  return _slashtargetPath + path.Substring(_slashname.Length);
    //}

    public static bool HasUniqueNames(Directory[] directories)
    {
      System.Collections.Specialized.StringCollection names =
        new System.Collections.Specialized.StringCollection();
      foreach (Directory dir in directories)
      {
        if (names.Contains(dir.Name))
          return false;
        names.Add(dir.Name);
      }
      return true;
    }
  }
}