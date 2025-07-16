using System;

namespace Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    public class TempTools {
  
        [MenuItem ("Tools/SortChildrenByName")]
	
        public static void SortChildrenByName() {
            foreach (var obj in Selection.gameObjects) {
                var children = new List<Transform>();
                for (var i = obj.transform.childCount - 1; i >= 0; i--) {
                    var child = obj.transform.GetChild(i);
                    children.Add(child);
                    child.parent = null;
                }
                children.Sort((t1,t2) => string.Compare(t1.name, t2.name, StringComparison.Ordinal));
                foreach (var child in children) {
                    child.parent = obj.transform;
                }
            }
        } // SortChildrenByName()
	
    } // class TempTools
}