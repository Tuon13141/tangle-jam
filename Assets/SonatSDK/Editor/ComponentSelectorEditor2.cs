using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.IO;


public class ComponentSelectorEditor2 : EditorWindow
{
    public enum TypeEnum
    {
        Any,
        Image,
        Sprite,
        ParticleSystem,
        Text,
        Button,
        TypeName,
        ToggleSprite,
        IndexSprite,
        Material,
    }

    [MenuItem("Assets/Clone To Streaming Assets")]
    public static void CloneToStreamming()
    {
        var selected = Selection.activeObject;
        string rootPath = AssetDatabase.GetAssetPath(selected);
        FileUtil.CopyFileOrDirectory(rootPath, string.Format("Assets/StreamingAssets/{0}", Path.GetFileName(rootPath)));
        Debug.Log(rootPath);
        Debug.Log(string.Format("Assets/StreamingAssets/{0}", Path.GetFileName(rootPath)));
        //if (Directory.Exists(selectionPath))
        //{
        //    // do something
        //}
    }

    [MenuItem("Tools/DuongPham/ComponentSelectorEditor2")]
    static void showEditor()
    {
        EditorWindow.GetWindow(typeof(ComponentSelectorEditor2), false, "Multiple Components Select");
    }

    GameObject[] targets;

    TypeEnum selectType
    {
        get { return _selectType; }
        set
        {
            if (value != _selectType)
            {
                _selectType = value;
                if (value != TypeEnum.TypeName)
                {
                    exactTypeName = String.Empty;
                }
            }
        }
    }

    string namePrefix;
    string textureNamePrefix;
    string classNamePrefix;
    string parentFolder;

    string exactTypeName
    {
        get { return _exactTypeName; }
        set
        {
            if (value != _exactTypeName)
            {
                _exactTypeName = value;
                if (value != String.Empty && value != "")
                {
                    selectType = TypeEnum.TypeName;
                }
            }
        }
    }

    TypeEnum _selectType;
    string _exactTypeName;

    private static string TextureFolder => "Assets/Arts/materials/textures/";
    private static string RawMaterialFolder => "Assets/Arts/materials/raw-materials/";
    private static string AtlasedMaterialFolder => "Assets/Arts/materials/atlased-materials/";

    int ignoreChange = 0;

    string TextureStartWith;

    void OnGUI()
    {
        if (ignoreChange == 0 && targets != null && targets.Length > 0)
        {
            GUILayout.Space(20);
            EditorGUIUtility.labelWidth = 120;
            GUILayout.BeginVertical(Level2);


            EditorGUILayout.LabelField(targets[0].name, NameStyle, new GUILayoutOption[] {GUILayout.Height(30)});

            if (exactTypeName != "")
                this.selectType = TypeEnum.TypeName;

            selectType = (TypeEnum) EditorGUILayout.EnumPopup("Select Type", this.selectType,
                new GUILayoutOption[] {GUILayout.Width(250)});
            this.exactTypeName = EditorGUILayout.TextField("Type Name", this.exactTypeName,
                new GUILayoutOption[] {GUILayout.Width(250)});

            GUILayout.Space(5);
            this.namePrefix = EditorGUILayout.TextField("Name Prefix", this.namePrefix,
                new GUILayoutOption[] {GUILayout.Width(250)});


            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Select", new GUILayoutOption[] {GUILayout.Width(50), GUILayout.Height(25)}))
            {
                selection.Clear();
                foreach (var target in targets)
                {
                    FindInGo(target);
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    Selection.objects = selection.ToArray();
                    _lastTargets = targets;
                    ignoreChange = 2;
                }
            }

            if (GUILayout.Button("Select Blank Image",
                new GUILayoutOption[] {GUILayout.Width(120), GUILayout.Height(25)}))
            {
                selection.Clear();
                foreach (var target in targets)
                {
                    FindInGo(target, TypeEnum.Image);
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    Selection.objects = selection.Where(x => x.GetComponent<Image>().sprite == null).ToArray();
                    _lastTargets = targets;
                    ignoreChange = 2;
                }
            }

            if (GUILayout.Button("Select UnEnable Image",
                new GUILayoutOption[] {GUILayout.Width(120), GUILayout.Height(25)}))
            {
                selection.Clear();
                foreach (var target in targets)
                {
                    FindInGo(target, TypeEnum.Image);
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    Selection.objects = selection.Where(x => x.GetComponent<Image>().enabled == false).ToArray();
                    _lastTargets = targets;
                    ignoreChange = 2;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
          
            SelectTextureOfImage();

            SelectTextureOfSprite();


            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Material", new GUILayoutOption[] {GUILayout.Width(120), GUILayout.Height(25)}))
            {
                selection.Clear();
                Directory.CreateDirectory(RawMaterialFolder);
                Directory.CreateDirectory(AtlasedMaterialFolder);
                Directory.CreateDirectory(TextureFolder);
                foreach (var target in targets)
                {
                    FindInGo(target, new[] {TypeEnum.Sprite, TypeEnum.ParticleSystem});
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    ignoreChange = 2;
                    _lastTargets = targets;
                    List<Texture> textures = new List<Texture>();
                    foreach (var gameObject in selection)
                    {
                        if (gameObject.GetComponent<SpriteRenderer>() != null)
                        {
                            var matertial = gameObject.GetComponent<SpriteRenderer>()?.sharedMaterial;
                            if (matertial != null && !matertial.name.StartsWith("Sprites-Default"))
                            {
                                //Debug.Log(gameObject + ": Material="+matertial, gameObject);
                                var texture = matertial.mainTexture;
                                if (!textures.Contains(texture))
                                {
                                    var path = AssetDatabase.GetAssetPath(texture);
                                    if (path.StartsWith("Assets"))
                                    {
                                        textures.Add(texture);
                                        Debug.Log(gameObject + "-" + texture.name, gameObject);
                                        Debug.Log(
                                            AssetDatabase.GetAssetPath(texture) + " : " + gameObject + " " +
                                            texture.name, texture);
                                        Debug.Log(AssetDatabase.GetAssetPath(texture) + "/" +
                                                  Path.GetFileName(AssetDatabase.GetAssetPath(texture)));
                                        //File.Copy(AssetDatabase.GetAssetPath(texture), LitJson.JsonMapper.ToJson(this));
                                    }
                                }
                            }
                        }

                        if (gameObject.GetComponent<ParticleSystem>() != null)
                        {
                            var material = gameObject.GetComponent<ParticleSystemRenderer>().sharedMaterial;
                            if (material != null && !material.name.StartsWith("Sprites-Default"))
                            {
                                var des = string.Format("{0}/{1}", RawMaterialFolder,
                                    Path.GetFileName(AssetDatabase.GetAssetPath(material)));
                                Debug.Log(gameObject, gameObject);
                                if (!File.Exists(des))
                                {
                                    File.Copy(AssetDatabase.GetAssetPath(material), des);


                                    //Debug.Log(gameObject + ": Material="+matertial, gameObject);
                                    var texture = material.mainTexture;
                                    if (!textures.Contains(texture))
                                    {
                                        var path = AssetDatabase.GetAssetPath(texture);
                                        if (path.StartsWith("Assets"))
                                        {
                                            textures.Add(texture);
                                            Debug.Log(gameObject + "-" + texture.name, gameObject);
                                            Debug.Log(
                                                AssetDatabase.GetAssetPath(texture) + " : " + gameObject + " " +
                                                texture.name, texture);

                                            var des2 = string.Format("{0}/{1}", TextureFolder,
                                                Path.GetFileName(AssetDatabase.GetAssetPath(texture)));
                                            if (!File.Exists(des2))
                                                File.Copy(AssetDatabase.GetAssetPath(texture), des2);
                                        }
                                    }
                                }

                                des = string.Format("{0}/{1}", AtlasedMaterialFolder,
                                    Path.GetFileName(AssetDatabase.GetAssetPath(material)));
                                if (File.Exists(des))
                                {
                                    var newCopy = AssetDatabase.LoadAssetAtPath<Material>(des);
                                    if (newCopy.mainTexture != null && newCopy.mainTexture.name == "effect-atlas")
                                        gameObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = newCopy;
                                    Debug.Log(newCopy, newCopy);
                                }
                            }

                            var trailMaterial = gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial;
                            if (trailMaterial != null && !trailMaterial.name.StartsWith("Sprites-Default"))
                            {
                                var des = string.Format("{0}/{1}", RawMaterialFolder,
                                    Path.GetFileName(AssetDatabase.GetAssetPath(trailMaterial)));
                                if (!File.Exists(des))
                                {
                                    File.Copy(AssetDatabase.GetAssetPath(trailMaterial), des);

                                    //Debug.Log(gameObject + ": Material="+matertial, gameObject);
                                    var texture = trailMaterial.mainTexture;
                                    if (!textures.Contains(texture))
                                    {
                                        var path = AssetDatabase.GetAssetPath(texture);
                                        if (path.StartsWith("Assets"))
                                        {
                                            textures.Add(texture);
                                            Debug.Log(gameObject + "-" + texture.name, gameObject);
                                            Debug.Log(
                                                AssetDatabase.GetAssetPath(texture) + " : " + gameObject + " " +
                                                texture.name, texture);

                                            var des2 = string.Format("{0}/{1}", TextureFolder,
                                                Path.GetFileName(AssetDatabase.GetAssetPath(texture)));
                                            if (!File.Exists(des2))
                                                File.Copy(AssetDatabase.GetAssetPath(texture), des2);
                                        }
                                    }
                                }

                                des = string.Format("{0}/{1}", AtlasedMaterialFolder,
                                    Path.GetFileName(AssetDatabase.GetAssetPath(trailMaterial)));
                                if (File.Exists(des))
                                {
                                    var newCopy = AssetDatabase.LoadAssetAtPath<Material>(des);
                                    if (newCopy.mainTexture != null && newCopy.mainTexture.name == "effect-atlas")
                                        gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = newCopy;
                                }
                            }
                        }
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log("textures count = " + textures.Count);
                    Selection.objects = textures.ToArray();
                }
            }

            if (GUILayout.Button("Select Material's Texture",
                new GUILayoutOption[] {GUILayout.Width(120), GUILayout.Height(25)}))
            {
                selection.Clear();
                Directory.CreateDirectory(RawMaterialFolder);
                Directory.CreateDirectory(TextureFolder);
                foreach (var target in targets)
                {
                    FindInGo(target, new[] {TypeEnum.Sprite, TypeEnum.ParticleSystem});
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    ignoreChange = 2;
                    _lastTargets = targets;
                    List<Texture> textures = new List<Texture>();
                    foreach (var gameObject in selection)
                    {
                        if (gameObject.GetComponent<SpriteRenderer>() != null)
                        {
                            var matertial = gameObject.GetComponent<SpriteRenderer>()?.sharedMaterial;
                            if (matertial != null && !matertial.name.StartsWith("Sprites-Default"))
                            {
                                //Debug.Log(gameObject + ": Material="+matertial, gameObject);
                                var texture = matertial.mainTexture;
                                if (!textures.Contains(texture))
                                {
                                    var path = AssetDatabase.GetAssetPath(texture);
                                    if (path.StartsWith("Assets"))
                                    {
                                        textures.Add(texture);
                                        Debug.Log(gameObject + "-" + texture.name, gameObject);
                                        Debug.Log(
                                            AssetDatabase.GetAssetPath(texture) + " : " + gameObject + " " +
                                            texture.name, texture);
                                        Debug.Log(AssetDatabase.GetAssetPath(texture) + "/" +
                                                  Path.GetFileName(AssetDatabase.GetAssetPath(texture)));
                                        //File.Copy(AssetDatabase.GetAssetPath(texture), LitJson.JsonMapper.ToJson(this));
                                    }
                                }
                            }
                        }

                        if (gameObject.GetComponent<ParticleSystem>() != null)
                        {
                            var material = gameObject.GetComponent<ParticleSystemRenderer>().sharedMaterial;
                            if (material != null && !material.name.StartsWith("Sprites-Default"))
                            {
                                //Debug.Log(gameObject + ": Material="+matertial, gameObject);
                                var texture = material.mainTexture;
                                if (!textures.Contains(texture))
                                {
                                    var path = AssetDatabase.GetAssetPath(texture);
                                    if (path.StartsWith("Assets"))
                                    {
                                        textures.Add(texture);
                                        Debug.Log(gameObject + "-" + texture.name, gameObject);
                                        Debug.Log(
                                            AssetDatabase.GetAssetPath(texture) + " : " + gameObject + " " +
                                            texture.name, texture);
                                    }
                                }
                            }

                            var trailMaterial = gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial;
                            if (trailMaterial != null && !trailMaterial.name.StartsWith("Sprites-Default"))
                            {
                                //Debug.Log(gameObject + ": Material="+matertial, gameObject);
                                var texture = trailMaterial.mainTexture;
                                if (!textures.Contains(texture))
                                {
                                    var path = AssetDatabase.GetAssetPath(texture);
                                    if (path.StartsWith("Assets"))
                                    {
                                        textures.Add(texture);
                                        Debug.Log(gameObject + "-" + texture.name, gameObject);
                                        Debug.Log(
                                            AssetDatabase.GetAssetPath(texture) + " : " + gameObject + " " +
                                            texture.name, texture);
                                    }
                                }
                            }
                        }
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log("textures count = " + textures.Count);
                    Selection.objects = textures.ToArray();
                }
            }


            GUILayout.EndHorizontal();


            GUILayout.Space(20);
            classNamePrefix = EditorGUILayout.TextField("Class Name Prefix", this.classNamePrefix,
                new GUILayoutOption[] {GUILayout.Width(250)});
            if (GUILayout.Button("Select by classname prefix",
                new GUILayoutOption[] {GUILayout.Width(200), GUILayout.Height(25)}))
            {
                if (classNamePrefix.Length > 0)
                {
                    selection.Clear();
                    foreach (var target in targets)
                    {
                        FindInGoByClassNamePrefix(target);
                    }

                    _notFound = selection.Count == 0;
                    if (!_notFound)
                    {
                        Selection.objects = selection.ToArray();
                        _lastTargets = targets;
                        ignoreChange = 2;
                    }
                }
            }

            if (GUILayout.Button("Select zero size RectTransform",
                new GUILayoutOption[] {GUILayout.Width(200), GUILayout.Height(25)}))
            {
                classNamePrefix = "RectTransform";
                if (classNamePrefix.Length > 0)
                {
                    selection.Clear();
                    foreach (var target in targets)
                    {
                        FindInGoByClassNamePrefix(target);
                    }

                    _notFound = selection.Count == 0;
                    selection.RemoveAll(x =>
                        x.GetComponent<RectTransform>().rect.size.x > 0.1f &&
                        x.GetComponent<RectTransform>().rect.size.y > 0.1f);
                    selection.ForEach(x => Debug.Log(x.GetComponent<RectTransform>().rect.size));
                    if (!_notFound)
                    {
                        Selection.objects = selection.ToArray();
                        _lastTargets = targets;
                        ignoreChange = 2;
                    }
                }
            }


            if (GUILayout.Button("Select all Texture2D",
                new GUILayoutOption[] {GUILayout.Width(200), GUILayout.Height(25)}))
            {
                SelectAllTexture("");
            }
            
            parentFolder = EditorGUILayout.TextField("Parent folder", this.parentFolder,
                new GUILayoutOption[] {GUILayout.Width(250)});
            if (GUILayout.Button("Select all Texture in folder",
                new GUILayoutOption[] {GUILayout.Width(200), GUILayout.Height(25)}))
            {
                SelectAllTexture(parentFolder);
            }



            if (GUILayout.Button("Load Sprite from 0 Temp",
                new GUILayoutOption[] {GUILayout.Width(200), GUILayout.Height(25)}))
            {
                selection.Clear();

                foreach (var target in targets)
                {
                    FindInGo(target, TypeEnum.Sprite);
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    _lastTargets = targets;
                    ignoreChange = 2;
                    foreach (var gameObject in selection)
                    {
                        var image = gameObject.GetComponent<SpriteRenderer>();
                        if (image.sprite != null)
                        {
                            var des = string.Format("{0}/{1}", "Assets/0 TempCopy/bullet",
                                Path.GetFileName(AssetDatabase.GetAssetPath(image.sprite.texture)));

                            Debug.Log(AssetDatabase.LoadAssetAtPath<Sprite>(des));
                            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(des);
                        }
                    }
                }
            }

            TextureStartWith = EditorGUILayout.TextField("TextureStartWith", this.TextureStartWith,
                new GUILayoutOption[] {GUILayout.Width(250)});
            if (GUILayout.Button("Select TextureStartWith",
                new GUILayoutOption[] {GUILayout.Width(200), GUILayout.Height(25)}))
            {
                selection.Clear();

                foreach (var target in targets)
                {
                    FindInGo(target, new[] {TypeEnum.Image, TypeEnum.Sprite,});
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    _lastTargets = targets;
                    ignoreChange = 2;
                    List<Texture> textures = new List<Texture>();
                    foreach (var gameObject in selection)
                    {
                        var image = gameObject.GetComponent<Image>();
                        if (image != null)
                        {
                            var sprite = image.sprite;
                            if (sprite != null)
                            {
                                if (sprite.texture.name.StartsWith(TextureStartWith))
                                {
                                    var path = AssetDatabase.GetAssetPath(sprite.texture);
                                    if (path.StartsWith("Assets"))
                                    {
                                        if (!textures.Contains(sprite.texture))
                                            Debug.Log(
                                                AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                                sprite.texture.name, gameObject);
                                        Debug.Log(
                                            AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                            sprite.texture.name, sprite.texture);
                                    }
                                }
                            }
                        }

                        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null)
                        {
                            var sprite = spriteRenderer.sprite;
                            if (sprite != null)
                            {
                                if (sprite.texture.name.StartsWith(TextureStartWith))
                                {
                                    var path = AssetDatabase.GetAssetPath(sprite.texture);
                                    if (path.StartsWith("Assets"))
                                    {
                                        if (!textures.Contains(sprite.texture))
                                            textures.Add(sprite.texture);
                                        Debug.Log(
                                            AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                            sprite.texture.name, gameObject);
                                        Debug.Log(
                                            AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                            sprite.texture.name, sprite.texture);
                                    }
                                }
                            }
                        }
                    }

                    Selection.objects = textures.ToArray();
                }
            }

            this.textureNamePrefix = EditorGUILayout.TextField("Texture Name Prefix", this.textureNamePrefix,
                new GUILayoutOption[] {GUILayout.Width(250)});
            if (GUILayout.Button("Select Texture with name Prefix",
                new GUILayoutOption[] {GUILayout.Width(300), GUILayout.Height(25)}))
            {
                selection.Clear();
                foreach (var target in targets)
                {
                    FindInGo(target, TypeEnum.Image);
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    Selection.objects = selection.Where(x => x.GetComponent<Image>().mainTexture != null
                                                             && x.GetComponent<Image>().mainTexture.name
                                                                 .StartsWith(textureNamePrefix)
                    ).ToArray();
                    _lastTargets = targets;
                    ignoreChange = 2;
                }
            }

            if (_notFound)
                EditorGUILayout.LabelField("Not found any !", NameStyle, new GUILayoutOption[] {GUILayout.Height(30)});
            
            
           
            
            GUILayout.EndVertical();
        }
        else
        {
            if (_lastTargets != null)
            {
                GUILayout.Space(20);
                EditorGUIUtility.labelWidth = 100;
                GUILayout.BeginVertical(Level2);
                GUILayout.Space(5);
                if (GUILayout.Button("Copllase", new GUILayoutOption[] {GUILayout.Width(120), GUILayout.Height(25)}))
                {
                    for (int i = 0; i < _lastTargets.Length; i++)
                    {
                        SetExpandedRecursive(_lastTargets[i], false);
                    }

                    Selection.objects = _lastTargets;
                }

                if (GUILayout.Button("Contains?", new GUILayoutOption[] {GUILayout.Width(120), GUILayout.Height(25)}))
                {
                    List<Text> texts = new List<Text>();
                    foreach (var o in Selection.objects)
                    {
                        var txt = (o as GameObject).GetComponent<Text>();
                        if (txt != null)
                            texts.Add(txt);
                    }

                    Debug.Log(texts.Count);
                    foreach (var text in texts)
                    {
                        if (text.text.Contains("Close"))
                            Debug.Log("found");
                    }

                  
                }

                GUILayout.EndVertical();
            }
        }
    }

    private void SelectTextureOfSprite()
    {
         if (GUILayout.Button("Texture of SpriteRenderer",
                      new GUILayoutOption[] {GUILayout.Width(120), GUILayout.Height(25)}))
                  {
                      selection.Clear();
      
                      foreach (var target in targets)
                      {
                          FindInGo(target, TypeEnum.Sprite);
                      }
      
                      _notFound = selection.Count == 0;
                      if (!_notFound)
                      {
                          _lastTargets = targets;
                          ignoreChange = 2;
                          List<Texture> textures = new List<Texture>();
                          foreach (var gameObject in selection)
                          {
                              var image = gameObject.GetComponent<SpriteRenderer>();
                              if (image != null)
                              {
                                  var sprite = image.sprite;
                                  if (sprite != null)
                                  {
                                      if (!textures.Contains(sprite.texture))
                                      {
                                          var path = AssetDatabase.GetAssetPath(sprite.texture);
                                          if (path.StartsWith("Assets"))
                                          {
                                              textures.Add(sprite.texture);
                                              Debug.Log(
                                                  AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                                  sprite.texture.name, sprite.texture);
                                              ;
                                          }
                                      }
                                  }
                              }
                          }
      
                          Selection.objects = textures.ToArray();
                      }
                  }
    }

    private void SelectTextureOfImage()
    {
         if (GUILayout.Button("Texture of Image",
                new GUILayoutOption[] {GUILayout.Width(120), GUILayout.Height(25)}))
            {
                selection.Clear();

                foreach (var target in targets)
                {
                    FindInGo(target, TypeEnum.Image);
                }

                foreach (var target in targets)
                {
                    FindInGo(target, TypeEnum.ToggleSprite);
                }

                foreach (var target in targets)
                {
                    FindInGo(target, TypeEnum.IndexSprite);
                }

                _notFound = selection.Count == 0;
                if (!_notFound)
                {
                    _lastTargets = targets;
                    ignoreChange = 2;
                    List<Texture> textures = new List<Texture>();
                    foreach (var gameObject in selection)
                    {
                        var image = gameObject.GetComponent<Image>();
                        if (image != null)
                        {
                            var sprite = image.sprite;
                            if (sprite != null && !sprite.texture.name.StartsWith("__"))
                            {
                                if (!textures.Contains(sprite.texture))
                                {
                                    var path = AssetDatabase.GetAssetPath(sprite.texture);
                                    if (path.StartsWith("Assets"))
                                    {
                                        textures.Add(sprite.texture);
                                        Debug.Log(path + " : " + gameObject + " " + sprite.texture.name, gameObject);
                                        Debug.Log(path + " : " + gameObject + " " + sprite.texture.name,
                                            sprite.texture);
                                    }
                                }
                            }
                        }
                    }
                    Selection.objects = textures.ToArray();
                }
            }
    }

    private void FindAllGoHaveTexture(GameObject g)
    {
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] != null && components[i].GetType() == typeof(Image))
                selection.Add(g);
            else if (components[i] != null && components[i].GetType() == typeof(SpriteRenderer))
                selection.Add(g);
//            else if (components[i] != null && components[i].GetType() == typeof(HoverSpriteChanger))
//                selection.Add(g);
//            else if (components[i] != null && components[i].GetType() == typeof(ToggleSound2))
//                selection.Add(g);
        }

        foreach (Transform childT in g.transform)
        {
            FindAllGoHaveTexture(childT.gameObject);
        }
    }

    private void SelectAllTexture(string contain)
    {
        selection.Clear();

        foreach (var target in targets)
            FindAllGoHaveTexture(target);

        _notFound = selection.Count == 0;
        if (!_notFound)
        {
            _lastTargets = targets;
            ignoreChange = 2;
            List<Texture> textures = new List<Texture>();
            foreach (var gameObject in selection)
            {
                var image = gameObject.GetComponent<Image>();
                if (image != null)
                {
                    var sprite = image.sprite;
                    if (sprite != null)
                    {
                        if (!textures.Contains(sprite.texture))
                        {
                            var path = AssetDatabase.GetAssetPath(sprite.texture);
                            if (path.StartsWith("Assets") && (string.IsNullOrEmpty(contain) || path.Contains(contain)))
                            {
                                textures.Add(sprite.texture);
                                Debug.Log(
                                    AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                    sprite.texture.name, gameObject);
                                Debug.Log(
                                    AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                    sprite.texture.name, sprite.texture);
                            }
                        }
                    }
                    else
                        Debug.LogError("Image's sprite is null", gameObject);
                }

                var renderer = gameObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    var sprite = renderer.sprite;
                    if (sprite != null)
                    {
                        if (!textures.Contains(sprite.texture))
                        {
                            var path = AssetDatabase.GetAssetPath(sprite.texture);
                            if (path.StartsWith("Assets")&& (string.IsNullOrEmpty(contain) || path.Contains(contain)))
                            {
                                textures.Add(sprite.texture);
                                Debug.Log(
                                    AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                    sprite.texture.name, gameObject);
                                Debug.Log(
                                    AssetDatabase.GetAssetPath(sprite.texture) + " : " + gameObject + " " +
                                    sprite.texture.name, sprite.texture);
                            }
                        }
                    }
                    else
                        Debug.LogError("SpriteRenderer's sprite is null", gameObject);
                }
            }

            Selection.objects = textures.ToArray();
        }
    }

    bool _notFound;
    private GameObject[] _lastTargets;

    [SerializeField] List<GameObject> selection = new List<GameObject>();

    private void FindInGo(GameObject g)
    {
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (selectType == TypeEnum.Image && components[i] != null && components[i].GetType() == typeof(Image))
                selection.Add(g);
            else if (selectType == TypeEnum.Sprite && components[i] != null &&
                     components[i].GetType() == typeof(SpriteRenderer))
                selection.Add(g);
            else if (selectType == TypeEnum.Text && components[i] != null && components[i].GetType() == typeof(Text))
                selection.Add(g);
            else if (selectType == TypeEnum.Button && components[i] != null &&
                     components[i].GetType() == typeof(Button))
                selection.Add(g);
            else if (selectType == TypeEnum.Any && StartWiths(g.name, namePrefix))
                selection.Add(g);
            else if (selectType == TypeEnum.TypeName && components[i] != null &&
                     components[i].GetType().Name.Contains(exactTypeName) && StartWiths(g.name, namePrefix))
                selection.Add(g);
        }

        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGo(childT.gameObject);
        }
    }

    private void FindInGo(GameObject g, TypeEnum type)
    {
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (type == TypeEnum.Image && components[i] != null && components[i].GetType() == typeof(Image))
                selection.Add(g);
            else if (type == TypeEnum.Sprite && components[i] != null &&
                     components[i].GetType() == typeof(SpriteRenderer))
                selection.Add(g);
            else if (type == TypeEnum.Text && components[i] != null && components[i].GetType() == typeof(Text))
                selection.Add(g);
            else if (type == TypeEnum.Button && components[i] != null && components[i].GetType() == typeof(Button))
                selection.Add(g);
            else if (type == TypeEnum.ParticleSystem && components[i] != null &&
                     components[i].GetType() == typeof(ParticleSystem))
                selection.Add(g);
            else if (type == TypeEnum.Any && StartWiths(g.name, namePrefix))
                selection.Add(g);
            else if (type == TypeEnum.TypeName && components[i] != null &&
                     components[i].GetType().Name.Contains(exactTypeName) && StartWiths(g.name, namePrefix))
                selection.Add(g);
        }

        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGo(childT.gameObject, type);
        }
    }

    private void FindInGo(GameObject g, TypeEnum[] types)
    {
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (types.Contains(TypeEnum.Image) && components[i] != null && components[i].GetType() == typeof(Image))
                selection.Add(g);
            else if (types.Contains(TypeEnum.Sprite) && components[i] != null &&
                     components[i].GetType() == typeof(SpriteRenderer))
                selection.Add(g);
            else if (types.Contains(TypeEnum.Text) && components[i] != null && components[i].GetType() == typeof(Text))
                selection.Add(g);
            else if (types.Contains(TypeEnum.Button) && components[i] != null &&
                     components[i].GetType() == typeof(Button))
                selection.Add(g);
            else if (types.Contains(TypeEnum.ParticleSystem) && components[i] != null &&
                     components[i].GetType() == typeof(ParticleSystem))
                selection.Add(g);
            else if (types.Contains(TypeEnum.Any) && StartWiths(g.name, namePrefix))
                selection.Add(g);
            else if (types.Contains(TypeEnum.TypeName) && components[i] != null &&
                     components[i].GetType().Name.Contains(exactTypeName) && StartWiths(g.name, namePrefix))
                selection.Add(g);
        }

        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGo(childT.gameObject, types);
        }
    }

    private void FindInGoByClassNamePrefix(GameObject g)
    {
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].GetType().Name.StartsWith(classNamePrefix))
                selection.Add(g);
        }

        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGoByClassNamePrefix(childT.gameObject);
        }
    }

    private static void SetExpandedRecursive(GameObject go, bool expand)
    {
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        var methodInfo = type.GetMethod("SetExpandedRecursive");

        EditorApplication.ExecuteMenuItem("Window/Hierarchy");
        var window = EditorWindow.focusedWindow;

        methodInfo.Invoke(window, new object[] {go.GetInstanceID(), expand});
    }

    private static bool StartWiths(string stringName, string prefixesByComma)
    {
        string[] prefixes = prefixesByComma.Split(',');
        foreach (var prefix in prefixes)
        {
            if (stringName.StartsWith(prefix))
                return true;
        }

        return false;
    }

    private void OnSelectionChange()
    {
        if (ignoreChange > 0)
            ignoreChange--;
        else
        {
            var selects = Selection.gameObjects;
            if (selects.Length > 0)
            {
                targets = selects;
            }
            else
                targets = null;

            _notFound = false;
        }

        this.Repaint();
    }

    #region Styles

    GUIStyle NameStyle
    {
        get
        {
            if (_gameObjectNameStyle == null)
            {
                _gameObjectNameStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState()
                    {
                        textColor = Color.blue,
                    },
                    fontSize = 15
                };
            }

            return _gameObjectNameStyle;
        }
    }

    GUIStyle _gameObjectNameStyle;


    GUIStyle Level1
    {
        get
        {
            return new GUIStyle()
            {
                padding = new RectOffset(10, 0, 0, 0),
                fontStyle = FontStyle.Bold,
            };
        }
    }

    GUIStyle Level2Title
    {
        get
        {
            return new GUIStyle()
            {
                padding = new RectOffset(20, 0, 5, 0),
                fontStyle = FontStyle.Bold
            };
        }
    }

    GUIStyle Level2
    {
        get
        {
            return new GUIStyle()
            {
                padding = new RectOffset(5, 0, 5, 0),
                //fontStyle = FontStyle.Bold
            };
        }
    }

    GUILayoutOption[] baseLayout = new GUILayoutOption[] {GUILayout.Width(100f)};

    #endregion
}