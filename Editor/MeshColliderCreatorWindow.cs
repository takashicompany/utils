#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace takashicompany.Unity.Editor
{
    public class MeshColliderCreatorWindow : EditorWindow
    {
        // 設定項目
        private Vector3Int _divisions = new Vector3Int(4, 4, 4);
        private ColliderType _colliderType = ColliderType.Box;
        private bool _enableMerge = true;

        // 内部状態
        private Vector2 _scrollPos;

        public enum ColliderType
        {
            Box,
            SphereAndCapsules
        }

        [MenuItem("TC Utils/Mesh Collider Creator")]
        public static void ShowWindow()
        {
            GetWindow<MeshColliderCreatorWindow>("Mesh Collider Creator");
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            GUILayout.Label("Mesh Collider Creator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("MeshRendererの形状からプリミティブコライダーを生成します。\n対象のMeshRendererがついたオブジェクトを選択してから実行してください。", MessageType.Info);
            EditorGUILayout.Space();

            // 設定
            EditorGUILayout.LabelField("設定", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            // XYZ分割数
            EditorGUILayout.LabelField("分割数");
            EditorGUI.indentLevel++;
            _divisions.x = EditorGUILayout.IntSlider("X", _divisions.x, 1, 32);
            _divisions.y = EditorGUILayout.IntSlider("Y", _divisions.y, 1, 32);
            _divisions.z = EditorGUILayout.IntSlider("Z", _divisions.z, 1, 32);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            // Colliderタイプ
            _colliderType = (ColliderType)EditorGUILayout.EnumPopup("Colliderタイプ", _colliderType);

            EditorGUILayout.Space();

            // マージオプション
            _enableMerge = EditorGUILayout.Toggle("グリッドをマージする", _enableMerge);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            // 選択状態の表示
            EditorGUILayout.LabelField("選択オブジェクト", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            var selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                var meshRenderer = selectedObject.GetComponent<MeshRenderer>();
                var meshFilter = selectedObject.GetComponent<MeshFilter>();

                EditorGUILayout.LabelField("オブジェクト名", selectedObject.name);

                if (meshRenderer != null && meshFilter != null && meshFilter.sharedMesh != null)
                {
                    EditorGUILayout.LabelField("メッシュ名", meshFilter.sharedMesh.name);
                    EditorGUILayout.LabelField("頂点数", meshFilter.sharedMesh.vertexCount.ToString());
                    EditorGUILayout.LabelField("三角形数", (meshFilter.sharedMesh.triangles.Length / 3).ToString());

                    var bounds = meshRenderer.bounds;
                    EditorGUILayout.LabelField("バウンズサイズ", bounds.size.ToString("F3"));
                }
                else
                {
                    EditorGUILayout.HelpBox("MeshRendererまたはMeshFilterがありません", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("オブジェクトが選択されていません", MessageType.Warning);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            // 実行ボタン
            GUI.enabled = CanExecute();
            if (GUILayout.Button("コライダーを生成", GUILayout.Height(40)))
            {
                ExecuteColliderCreation();
            }
            GUI.enabled = true;

            EditorGUILayout.Space();

            // 既存のコライダー子オブジェクトを削除するボタン
            if (selectedObject != null)
            {
                if (GUILayout.Button("生成したコライダーを削除", GUILayout.Height(30)))
                {
                    RemoveGeneratedColliders(selectedObject);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private bool CanExecute()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null) return false;

            var meshRenderer = selectedObject.GetComponent<MeshRenderer>();
            var meshFilter = selectedObject.GetComponent<MeshFilter>();

            return meshRenderer != null && meshFilter != null && meshFilter.sharedMesh != null;
        }

        private void ExecuteColliderCreation()
        {
            var selectedObject = Selection.activeGameObject;
            var meshFilter = selectedObject.GetComponent<MeshFilter>();
            var meshRenderer = selectedObject.GetComponent<MeshRenderer>();

            Undo.RegisterFullObjectHierarchyUndo(selectedObject, "Create Mesh Colliders");

            // 1. 既存の生成済みコライダーを削除
            RemoveGeneratedColliders(selectedObject);

            // 2. メッシュ情報を取得
            var mesh = meshFilter.sharedMesh;
            var bounds = meshRenderer.bounds;

            // 3. グリッドを作成し、メッシュとの交差判定を行う
            var gridData = CreateGridData(selectedObject.transform, mesh, bounds);

            // 4. 有効なグリッドをマージ
            var mergedRegions = MergeGrids(gridData);

            // 5. コライダーを生成
            CreateColliders(selectedObject, mergedRegions, bounds);

            EditorUtility.DisplayDialog("完了", $"{mergedRegions.Count}個のコライダーを生成しました", "OK");
        }

        private bool[,,] CreateGridData(Transform transform, Mesh mesh, Bounds worldBounds)
        {
            var gridData = new bool[_divisions.x, _divisions.y, _divisions.z];

            // ワールド座標のバウンズからローカル座標に変換
            var localBoundsCenter = transform.InverseTransformPoint(worldBounds.center);
            var localBoundsSize = transform.InverseTransformVector(worldBounds.size);
            // InverseTransformVectorはスケールの影響を受けるので、符号を調整
            localBoundsSize = new Vector3(
                Mathf.Abs(localBoundsSize.x),
                Mathf.Abs(localBoundsSize.y),
                Mathf.Abs(localBoundsSize.z)
            );

            var localBounds = new Bounds(localBoundsCenter, localBoundsSize);

            // メッシュの頂点と三角形を取得
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;

            // 各グリッドセルについて判定
            var gridSize = new Vector3(
                localBounds.size.x / _divisions.x,
                localBounds.size.y / _divisions.y,
                localBounds.size.z / _divisions.z
            );

            for (int x = 0; x < _divisions.x; x++)
            {
                for (int y = 0; y < _divisions.y; y++)
                {
                    for (int z = 0; z < _divisions.z; z++)
                    {
                        // グリッドセルのバウンズを計算（ローカル座標）
                        var cellMin = localBounds.min + new Vector3(
                            x * gridSize.x,
                            y * gridSize.y,
                            z * gridSize.z
                        );
                        var cellCenter = cellMin + gridSize * 0.5f;
                        var cellBounds = new Bounds(cellCenter, gridSize);

                        // このセルにメッシュが存在するか判定
                        gridData[x, y, z] = DoesMeshIntersectBox(vertices, triangles, cellBounds);
                    }
                }
            }

            return gridData;
        }

        private bool DoesMeshIntersectBox(Vector3[] vertices, int[] triangles, Bounds box)
        {
            // 三角形ごとに判定
            for (int i = 0; i < triangles.Length; i += 3)
            {
                var v0 = vertices[triangles[i]];
                var v1 = vertices[triangles[i + 1]];
                var v2 = vertices[triangles[i + 2]];

                // 三角形がボックスと交差するか判定
                if (TriangleBoxIntersect(v0, v1, v2, box))
                {
                    return true;
                }
            }

            return false;
        }

        // 三角形とAABBの交差判定（Separating Axis Theorem）
        private bool TriangleBoxIntersect(Vector3 v0, Vector3 v1, Vector3 v2, Bounds box)
        {
            // まず、三角形のバウンディングボックスとの判定
            var triMin = Vector3.Min(Vector3.Min(v0, v1), v2);
            var triMax = Vector3.Max(Vector3.Max(v0, v1), v2);

            if (triMax.x < box.min.x || triMin.x > box.max.x) return false;
            if (triMax.y < box.min.y || triMin.y > box.max.y) return false;
            if (triMax.z < box.min.z || triMin.z > box.max.z) return false;

            // 頂点がボックス内にあるか
            if (box.Contains(v0) || box.Contains(v1) || box.Contains(v2))
            {
                return true;
            }

            // 三角形の辺がボックスと交差するか
            if (LineIntersectsBox(v0, v1, box) ||
                LineIntersectsBox(v1, v2, box) ||
                LineIntersectsBox(v2, v0, box))
            {
                return true;
            }

            // ボックスの中心が三角形の平面上でボックス内にあるかどうか
            // (より厳密なSAT判定)
            var center = box.center;
            var extents = box.extents;

            // 三角形を原点中心に移動
            var tv0 = v0 - center;
            var tv1 = v1 - center;
            var tv2 = v2 - center;

            // 三角形のエッジ
            var e0 = tv1 - tv0;
            var e1 = tv2 - tv1;
            var e2 = tv0 - tv2;

            // 三角形の法線
            var normal = Vector3.Cross(e0, e1);

            // 平面との交差判定
            float r = extents.x * Mathf.Abs(normal.x) +
                     extents.y * Mathf.Abs(normal.y) +
                     extents.z * Mathf.Abs(normal.z);
            float s = Vector3.Dot(normal, tv0);

            if (Mathf.Abs(s) > r)
            {
                return false;
            }

            return true;
        }

        private bool LineIntersectsBox(Vector3 p1, Vector3 p2, Bounds box)
        {
            var dir = p2 - p1;
            var length = dir.magnitude;
            if (length < 0.0001f) return box.Contains(p1);

            dir /= length;

            float tmin = 0;
            float tmax = length;

            for (int i = 0; i < 3; i++)
            {
                float min = GetComponent(box.min, i);
                float max = GetComponent(box.max, i);
                float origin = GetComponent(p1, i);
                float direction = GetComponent(dir, i);

                if (Mathf.Abs(direction) < 0.0001f)
                {
                    if (origin < min || origin > max)
                        return false;
                }
                else
                {
                    float t1 = (min - origin) / direction;
                    float t2 = (max - origin) / direction;

                    if (t1 > t2)
                    {
                        float temp = t1;
                        t1 = t2;
                        t2 = temp;
                    }

                    tmin = Mathf.Max(tmin, t1);
                    tmax = Mathf.Min(tmax, t2);

                    if (tmin > tmax)
                        return false;
                }
            }

            return true;
        }

        private float GetComponent(Vector3 v, int index)
        {
            switch (index)
            {
                case 0: return v.x;
                case 1: return v.y;
                case 2: return v.z;
                default: return 0;
            }
        }

        private List<MergedRegion> MergeGrids(bool[,,] gridData)
        {
            var regions = new List<MergedRegion>();
            var visited = new bool[_divisions.x, _divisions.y, _divisions.z];

            if (!_enableMerge)
            {
                // マージしない場合：各グリッドを個別のコライダーとして扱う
                regions = CreateIndividualRegions(gridData, visited);
            }
            else if (_colliderType == ColliderType.Box)
            {
                // Boxコライダーの場合：直方体でマージ
                regions = MergeAsBoxes(gridData, visited);
            }
            else
            {
                // SphereAndCapsulesの場合：Capsuleでマージ
                regions = MergeAsCapsules(gridData, visited);
            }

            return regions;
        }

        private List<MergedRegion> CreateIndividualRegions(bool[,,] gridData, bool[,,] visited)
        {
            var regions = new List<MergedRegion>();

            for (int x = 0; x < _divisions.x; x++)
            {
                for (int y = 0; y < _divisions.y; y++)
                {
                    for (int z = 0; z < _divisions.z; z++)
                    {
                        if (gridData[x, y, z] && !visited[x, y, z])
                        {
                            visited[x, y, z] = true;

                            RegionType type;
                            if (_colliderType == ColliderType.Box)
                            {
                                type = RegionType.Box;
                            }
                            else
                            {
                                type = RegionType.Sphere;
                            }

                            regions.Add(new MergedRegion
                            {
                                startX = x,
                                startY = y,
                                startZ = z,
                                sizeX = 1,
                                sizeY = 1,
                                sizeZ = 1,
                                type = type
                            });
                        }
                    }
                }
            }

            return regions;
        }

        private List<MergedRegion> MergeAsBoxes(bool[,,] gridData, bool[,,] visited)
        {
            var regions = new List<MergedRegion>();

            // 貪欲法で直方体領域をマージ
            for (int x = 0; x < _divisions.x; x++)
            {
                for (int y = 0; y < _divisions.y; y++)
                {
                    for (int z = 0; z < _divisions.z; z++)
                    {
                        if (gridData[x, y, z] && !visited[x, y, z])
                        {
                            // 最大の直方体を探す
                            var region = FindLargestBox(gridData, visited, x, y, z);
                            regions.Add(region);

                            // 領域をマーク
                            for (int dx = 0; dx < region.sizeX; dx++)
                            {
                                for (int dy = 0; dy < region.sizeY; dy++)
                                {
                                    for (int dz = 0; dz < region.sizeZ; dz++)
                                    {
                                        visited[x + dx, y + dy, z + dz] = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return regions;
        }

        private MergedRegion FindLargestBox(bool[,,] gridData, bool[,,] visited, int startX, int startY, int startZ)
        {
            int maxSizeX = 1, maxSizeY = 1, maxSizeZ = 1;
            int bestVolume = 1;
            int bestSizeX = 1, bestSizeY = 1, bestSizeZ = 1;

            // X方向の最大拡張
            while (startX + maxSizeX < _divisions.x &&
                   gridData[startX + maxSizeX, startY, startZ] &&
                   !visited[startX + maxSizeX, startY, startZ])
            {
                maxSizeX++;
            }

            // Y方向の最大拡張
            while (startY + maxSizeY < _divisions.y)
            {
                bool canExtend = true;
                for (int dx = 0; dx < maxSizeX; dx++)
                {
                    if (!gridData[startX + dx, startY + maxSizeY, startZ] ||
                        visited[startX + dx, startY + maxSizeY, startZ])
                    {
                        canExtend = false;
                        break;
                    }
                }
                if (!canExtend) break;
                maxSizeY++;
            }

            // Z方向の最大拡張
            while (startZ + maxSizeZ < _divisions.z)
            {
                bool canExtend = true;
                for (int dx = 0; dx < maxSizeX; dx++)
                {
                    for (int dy = 0; dy < maxSizeY; dy++)
                    {
                        if (!gridData[startX + dx, startY + dy, startZ + maxSizeZ] ||
                            visited[startX + dx, startY + dy, startZ + maxSizeZ])
                        {
                            canExtend = false;
                            break;
                        }
                    }
                    if (!canExtend) break;
                }
                if (!canExtend) break;
                maxSizeZ++;
            }

            // 最大体積の組み合わせを探す
            for (int sx = 1; sx <= maxSizeX; sx++)
            {
                for (int sy = 1; sy <= maxSizeY; sy++)
                {
                    for (int sz = 1; sz <= maxSizeZ; sz++)
                    {
                        // この組み合わせが有効か確認
                        bool valid = true;
                        for (int dx = 0; dx < sx && valid; dx++)
                        {
                            for (int dy = 0; dy < sy && valid; dy++)
                            {
                                for (int dz = 0; dz < sz && valid; dz++)
                                {
                                    if (!gridData[startX + dx, startY + dy, startZ + dz] ||
                                        visited[startX + dx, startY + dy, startZ + dz])
                                    {
                                        valid = false;
                                    }
                                }
                            }
                        }

                        if (valid)
                        {
                            int volume = sx * sy * sz;
                            if (volume > bestVolume)
                            {
                                bestVolume = volume;
                                bestSizeX = sx;
                                bestSizeY = sy;
                                bestSizeZ = sz;
                            }
                        }
                    }
                }
            }

            return new MergedRegion
            {
                startX = startX,
                startY = startY,
                startZ = startZ,
                sizeX = bestSizeX,
                sizeY = bestSizeY,
                sizeZ = bestSizeZ,
                type = RegionType.Box
            };
        }

        private List<MergedRegion> MergeAsCapsules(bool[,,] gridData, bool[,,] visited)
        {
            var regions = new List<MergedRegion>();

            // 各軸方向に連続するグリッドをCapsuleとしてマージ
            for (int x = 0; x < _divisions.x; x++)
            {
                for (int y = 0; y < _divisions.y; y++)
                {
                    for (int z = 0; z < _divisions.z; z++)
                    {
                        if (gridData[x, y, z] && !visited[x, y, z])
                        {
                            // 各軸方向で最も長いカプセルを探す
                            int lengthX = GetLineLength(gridData, visited, x, y, z, 1, 0, 0);
                            int lengthY = GetLineLength(gridData, visited, x, y, z, 0, 1, 0);
                            int lengthZ = GetLineLength(gridData, visited, x, y, z, 0, 0, 1);

                            MergedRegion region;

                            // 最も長い方向を選択
                            if (lengthY >= lengthX && lengthY >= lengthZ && lengthY > 1)
                            {
                                // Y軸方向のCapsule
                                region = new MergedRegion
                                {
                                    startX = x,
                                    startY = y,
                                    startZ = z,
                                    sizeX = 1,
                                    sizeY = lengthY,
                                    sizeZ = 1,
                                    type = RegionType.CapsuleY
                                };
                                for (int i = 0; i < lengthY; i++)
                                    visited[x, y + i, z] = true;
                            }
                            else if (lengthX >= lengthZ && lengthX > 1)
                            {
                                // X軸方向のCapsule
                                region = new MergedRegion
                                {
                                    startX = x,
                                    startY = y,
                                    startZ = z,
                                    sizeX = lengthX,
                                    sizeY = 1,
                                    sizeZ = 1,
                                    type = RegionType.CapsuleX
                                };
                                for (int i = 0; i < lengthX; i++)
                                    visited[x + i, y, z] = true;
                            }
                            else if (lengthZ > 1)
                            {
                                // Z軸方向のCapsule
                                region = new MergedRegion
                                {
                                    startX = x,
                                    startY = y,
                                    startZ = z,
                                    sizeX = 1,
                                    sizeY = 1,
                                    sizeZ = lengthZ,
                                    type = RegionType.CapsuleZ
                                };
                                for (int i = 0; i < lengthZ; i++)
                                    visited[x, y, z + i] = true;
                            }
                            else
                            {
                                // 単独グリッドはSphere
                                region = new MergedRegion
                                {
                                    startX = x,
                                    startY = y,
                                    startZ = z,
                                    sizeX = 1,
                                    sizeY = 1,
                                    sizeZ = 1,
                                    type = RegionType.Sphere
                                };
                                visited[x, y, z] = true;
                            }

                            regions.Add(region);
                        }
                    }
                }
            }

            return regions;
        }

        private int GetLineLength(bool[,,] gridData, bool[,,] visited, int x, int y, int z, int dx, int dy, int dz)
        {
            int length = 0;
            while (x + dx * length >= 0 && x + dx * length < _divisions.x &&
                   y + dy * length >= 0 && y + dy * length < _divisions.y &&
                   z + dz * length >= 0 && z + dz * length < _divisions.z &&
                   gridData[x + dx * length, y + dy * length, z + dz * length] &&
                   !visited[x + dx * length, y + dy * length, z + dz * length])
            {
                length++;
            }
            return length;
        }

        private void CreateColliders(GameObject parent, List<MergedRegion> regions, Bounds worldBounds)
        {
            var parentTransform = parent.transform;

            // ワールド座標からローカル座標へ
            var localBoundsCenter = parentTransform.InverseTransformPoint(worldBounds.center);
            var localBoundsSize = parentTransform.InverseTransformVector(worldBounds.size);
            localBoundsSize = new Vector3(
                Mathf.Abs(localBoundsSize.x),
                Mathf.Abs(localBoundsSize.y),
                Mathf.Abs(localBoundsSize.z)
            );

            var localBounds = new Bounds(localBoundsCenter, localBoundsSize);

            var gridSize = new Vector3(
                localBounds.size.x / _divisions.x,
                localBounds.size.y / _divisions.y,
                localBounds.size.z / _divisions.z
            );

            // 親となるGameObjectを作成
            var colliderParent = new GameObject("_GeneratedColliders");
            colliderParent.transform.SetParent(parentTransform, false);
            Undo.RegisterCreatedObjectUndo(colliderParent, "Create Collider Parent");

            int colliderIndex = 0;
            foreach (var region in regions)
            {
                // 領域の中心とサイズを計算（ローカル座標）
                var regionMin = localBounds.min + new Vector3(
                    region.startX * gridSize.x,
                    region.startY * gridSize.y,
                    region.startZ * gridSize.z
                );
                var regionSize = new Vector3(
                    region.sizeX * gridSize.x,
                    region.sizeY * gridSize.y,
                    region.sizeZ * gridSize.z
                );
                var regionCenter = regionMin + regionSize * 0.5f;

                var colliderObject = new GameObject($"Collider_{colliderIndex++}");
                colliderObject.transform.SetParent(colliderParent.transform, false);
                colliderObject.transform.localPosition = regionCenter;

                switch (region.type)
                {
                    case RegionType.Box:
                        var boxCollider = colliderObject.AddComponent<BoxCollider>();
                        boxCollider.size = regionSize;
                        break;

                    case RegionType.Sphere:
                        var sphereCollider = colliderObject.AddComponent<SphereCollider>();
                        // 球の半径は最小の軸のサイズの半分
                        sphereCollider.radius = Mathf.Min(gridSize.x, Mathf.Min(gridSize.y, gridSize.z)) * 0.5f;
                        break;

                    case RegionType.CapsuleX:
                        var capsuleX = colliderObject.AddComponent<CapsuleCollider>();
                        capsuleX.direction = 0; // X軸
                        capsuleX.radius = Mathf.Min(gridSize.y, gridSize.z) * 0.5f;
                        capsuleX.height = regionSize.x;
                        break;

                    case RegionType.CapsuleY:
                        var capsuleY = colliderObject.AddComponent<CapsuleCollider>();
                        capsuleY.direction = 1; // Y軸
                        capsuleY.radius = Mathf.Min(gridSize.x, gridSize.z) * 0.5f;
                        capsuleY.height = regionSize.y;
                        break;

                    case RegionType.CapsuleZ:
                        var capsuleZ = colliderObject.AddComponent<CapsuleCollider>();
                        capsuleZ.direction = 2; // Z軸
                        capsuleZ.radius = Mathf.Min(gridSize.x, gridSize.y) * 0.5f;
                        capsuleZ.height = regionSize.z;
                        break;
                }

                Undo.RegisterCreatedObjectUndo(colliderObject, "Create Collider");
            }
        }

        private void RemoveGeneratedColliders(GameObject parent)
        {
            var existingColliders = parent.transform.Find("_GeneratedColliders");
            if (existingColliders != null)
            {
                Undo.DestroyObjectImmediate(existingColliders.gameObject);
            }
        }

        private struct MergedRegion
        {
            public int startX, startY, startZ;
            public int sizeX, sizeY, sizeZ;
            public RegionType type;
        }

        private enum RegionType
        {
            Box,
            Sphere,
            CapsuleX,
            CapsuleY,
            CapsuleZ
        }
    }
}
#endif
