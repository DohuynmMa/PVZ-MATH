using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
namespace Assets.Scripts.Utils
{
    public static class Utils
    {
        public static string scene
        {
            get
            {
                return SceneManager.GetActiveScene().name;
            }
        }
        /// <summary>
        /// 是否为小数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool numIsFloat(this float num)
        {
            return num - Mathf.Floor(num) != 0;
        }
        /// <summary>
        /// 选出min和max中离它最近的数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float whatsTheNumNear(this float num,float min,float max)
        {
            return Mathf.Abs(num - min) >= Mathf.Abs(num - max) ? max : min;
        }
        /// <summary>
        /// 是否在某个范围内
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool numInRange(this float num, float min, float max)
        {
            return num <= max && num >= min;
        }
        /// <summary>
        /// 把实体移动到鼠标(点击位置)
        /// </summary>
        /// <param name="entity"></param>
        public static void moveToMouse(this GameObject obj, float offsetX = 0, float offsetY = 0)
        {
            if (obj == null) return;

            // 创建一个位于Z=0的平面（法线方向为Z轴正方向）
            Plane zZeroPlane = new Plane(Vector3.forward, Vector3.zero);

            // 从相机通过鼠标位置发射一条射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 计算射线与Z=0平面的交点
            if (zZeroPlane.Raycast(ray, out float enter))
            {
                Vector3 mouseWorldPosition = ray.GetPoint(enter);

                // 应用偏移
                mouseWorldPosition.x += offsetX;
                mouseWorldPosition.y += offsetY;

                // 设置物体位置（强制Z=0）
                obj.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
            }
        }
        /// <summary>
        /// 移动UI到transform的坐标
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="pos"></param>
        public static void uiMove(GameObject ui, Vector3 pos)
        {
            if (ui == null) return;
            var mScreenPos = Camera.main.WorldToScreenPoint(pos);
            Vector2 mRectPos;
            if (ui.transform.parent.GetComponent<Canvas>() == null)
            {
                Debug.Log("null uiParent");
                return;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(ui.transform.parent.GetComponent<Canvas>().GetComponent<RectTransform>(), mScreenPos, null, out mRectPos);
            ui.GetComponent<RectTransform>().anchoredPosition = mRectPos;
        }
        /// 获取UI元素在世界空间中的位置
        /// </summary>
        /// <param name="uiElement">目标UI元素</param>
        /// <returns>UI元素在世界空间中的位置</returns>
        public static Vector3 GetUIWorldPosition(this GameObject ue)
        {
            var uiElement = ue.GetComponent<RectTransform>();
            if (uiElement == null)
            {
                Debug.LogError("UI元素不能为空");
                return Vector3.zero;
            }
            return uiElement.position;
        }
        public static void MoveToUI(Transform objectToMove, RectTransform targetUI, float moveSpeed = 5f)
        {
            if (objectToMove == null || targetUI == null)
            {
                Debug.LogError("物体或UI元素不能为空");
                return;
            }
            Camera mainCamera = Camera.main ?? Camera.current;
            if (mainCamera == null)
            {
                Debug.LogError("找不到可用的相机");
                return;
            }
            Vector2 uiScreenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, targetUI.position);
            float distance = Vector3.Distance(mainCamera.transform.position, objectToMove.position);
            Vector3 targetWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(uiScreenPos.x, uiScreenPos.y, distance));
            objectToMove.position = Vector3.MoveTowards(objectToMove.position, targetWorldPos, moveSpeed * Time.deltaTime);
        }
        /// <summary>
        /// 得到实体碰撞体中心
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Vector3 getEntityBoxColliderPos(this Entity entity)
        {
            if (entity == null) return Vector3.zero;
            var colliderPos2 = entity.bc.bounds.center;
            var currentPos = new Vector3(colliderPos2.x, colliderPos2.y, entity.transform.position.z);
            return currentPos;
        }
        /// <summary>
        /// 得到所有格子
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static List<Cell> findAllCells(int quadrant = 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Cell");
            var cells = new List<Cell>();
            foreach (var obj in objs)
            {
                if (obj == null) continue;
                var cell = obj.GetComponent<Cell>();
                if(cell == null) continue;
                if (quadrant != 0 && cell.quadrant != quadrant) continue;
                cells.Add(cell);
            }
            return cells;
        }
        public static List<Cell> findAllCellsByRow(int row,int quadrant = 1)
        {
            List<Cell> cells = new List<Cell>();
            foreach(var cell in findAllCells())
            {
                if (getCellRowByCellId(cell.cellId) == row && cell.quadrant == quadrant)
                {
                    cells.Add(cell);
                }
            }
            return cells;
        }
        public static List<Cell> findAllCellsByColumn(int column,int quadrant = 1)
        {
            List<Cell> cells = new List<Cell>();
            foreach (var cell in findAllCells())
            {
                if (getCellColumnByCellId(cell.cellId) == column && cell.quadrant == quadrant)
                {
                    cells.Add(cell);
                }
            }
            return cells;
        }
        /// <summary>
        /// 通过id找到cell
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Cell findCellById(int id,int quadrant = 1)
        {
            foreach(var cell in findAllCells())
            {
                if(cell.cellId == id && cell.quadrant == quadrant) return cell;
            }
            return null;
        }
        /// <summary>
        /// 找到所有实体
        /// </summary>
        /// <returns></returns>
        public static List<Entity> findAllEntities()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Entity");
            List<Entity> entities = new List<Entity>();
            foreach (var obj in objs)
            {
                if (obj.GetComponent<Entity>() == null) continue;
                var entity = obj.GetComponent<Entity>();
                entities.Add(entity);
            }
            return entities;
        }
        public static List<Entity> findAllEntities(EntityGroup group)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Entity");
            List<Entity> entities = new List<Entity>();
            foreach (var obj in objs)
            {
                if (obj.GetComponent<Entity>() == null) continue;
                var entity = obj.GetComponent<Entity>();
                if(entity.entityGroup != group) continue;
                entities.Add(entity);
            }
            return entities;
        }
        public static List<Entity> findAllEntitiesByRow(int row)
        {
            List<Entity> entities = new List<Entity>();
            foreach(var entity in findAllEntities())
            {
                if (!entities.Contains(entity) && entity.row == row)
                {
                    entities.Add(entity);
                }
            }
            return entities;
        }
        /// <summary>
        /// 检测物体内部所有物体的SpriteRenderer
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SpriteRenderer[] getAllSR(this GameObject obj)
        {
            var sr = obj.GetComponentsInChildren<SpriteRenderer>(true);
            if (obj.transform.childCount != 0)
            {
                List<SpriteRenderer> sr2 = sr.ToList();
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    sr2.AddRange(getAllSR(obj.transform.GetChild(i).gameObject).ToList());
                }
                return sr2.ToArray();
            }
            else return sr;
        }
        /// <summary>
        /// 根据TYPE改变当前背景
        /// </summary>
        /// <param name="type"></param>
        public static void changeBackground(BackgroundType type)
        {
            //删除原来的背景
            var objs = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (var b in objs)
            {
                if (b.GetComponent<Background>() != null)
                {
                    ResourceManager.Destroy(b);
                }
            }
            //实例化新背景
            var bg = findBackgroundPrefabByType(type);
            GameManager.Instance.currentBg = type;
            GameObject.Instantiate(bg,Vector3.zero,Quaternion.identity);
        }
        /// <summary>
        /// 删除全部背景
        /// </summary>
        public static void removeAllBackgrounds()
        {
            var objs = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (var b in objs)
            {
                if (b.GetComponent<Background>() != null)
                {
                    ResourceManager.Destroy(b);
                }
            }
        }
        /// <summary>
        /// 通过type找到实体预制体
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Entity findEntityPrefabByType(EntityType type)
        {
            var rm = ResourceManager.Instance;
            foreach(var entity in rm.entities)
            {
                if(entity.entityType == type) return entity;
            }
            return null;
        }
        /// <summary>
        /// 通过type找到子弹预制体
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Bullet findBulletPrefabByType(BulletType type)
        {
            var rm = ResourceManager.Instance;
            foreach (var b in rm.bullets)
            {
                if (b.bulletType == type) return b;
            }
            return null;
        }
        /// <summary>
        /// 通过type找到背景预制体
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Background findBackgroundPrefabByType(BackgroundType type)
        {
            var rm = ResourceManager.Instance;
            foreach (var b in rm.backgrounds)
            {
                if (b.backgroundType == type) return b;
            }
            return null;
        }
        /// <summary>
        /// 通过type找到Card
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Card findCardPrefabByType(CardType type)
        {
            var rm = ResourceManager.Instance;
            foreach (var c in rm.cards)
            {
                if (c.cardType == type) return c;
            }
            return null;
        }
        /// <summary>
        /// 通过type找到Card
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Effect findEffectPrefabByType(EffectType type)
        {
            var rm = ResourceManager.Instance;
            foreach (var e in rm.effects)
            {
                if (e.effectType == type) return e;
            }
            return null;
        }
        public static Effect summonEffectDirectly(EffectType type ,Vector3 pos)
        {
            var e = GameObject.Instantiate(findEffectPrefabByType(type), pos, Quaternion.identity).GetComponent<Effect>();
            var gm = GameManager.Instance;
            if (gm.inGame)
            {
                gm.effectsInGame.Add(e);
            }
            return e;
        }
        /// <summary>
        /// 直接放置T实体
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="cellId"></param>
        public static Entity putThinkEntityDirectly(EntityType entityType,int cellId,EntityGroup entityGroup = EntityGroup.Own)
        {
            if (findCellById(cellId).currentEntity != null) return null;
            var s = GameObject.Instantiate(findEntityPrefabByType(entityType), findCellById(cellId).GetComponent<BoxCollider2D>().bounds.center, Quaternion.identity);
            s.transitionToEnable();
            s.entityGroup = entityGroup;
            return s;
        }
        /// <summary>
        /// 直接生成P实体
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="row"></param>
        /// <param name="specPos"></param>
        /// <param name="specPos"></param>
        /// <returns></returns>
        public static ProblemEntity putProblemEntityDirectly(EntityType entityType, string difficultF = "EAZY", int difficultB = 0, int row = 1, int quadrant = 1,bool participateInCounting = true)
        {
            var accuratePos = new Vector3(18 * (quadrant == 2 || quadrant == 3 ? -1 : 1), (8.1f - (6 - Mathf.Abs(row)) * 1.3f) * (quadrant == 2 || quadrant == 1 ? 1 : -1) - (quadrant == 3 || quadrant == 4 ? 0.2f : 0), 0);
            var problem = GameObject.Instantiate(findEntityPrefabByType(entityType), accuratePos, Quaternion.identity).GetComponent<ProblemEntity>();
            problem.entityGroup = EntityGroup.Enemy;
            problem.transitionToEnable();
            problem.row = row;
            problem.currentQuadrant = quadrant;
            problem.checkQuadrantAndFlipX();
            if(participateInCounting) GameManager.Instance.summonedProblemCount++;
            return problem;
        }
        public static ProblemEntity putProblemEntityDirectly(EntityType entityType, string difficultF, int difficultB, int row,Vector3 specPos, bool participateInCounting = true)
        {
            var entity = putProblemEntityDirectly(entityType, difficultF, difficultB, row, 0,participateInCounting);
            entity.transform.position = specPos;
            if (entity != null) return entity;
            else return null;
        }
        /// <summary>
        /// 通过格子ID得到格子行数
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        public static int getCellRowByCellId(int cellId,int quadrant = 1)
        {
            var r1 = (int)(quadrant == 1 || quadrant == 2 ? (7 - Mathf.Ceil((float)cellId / 11)) : Mathf.Ceil((float)cellId / 11) * -1);
            return r1;
        }
        /// <summary>
        /// 通过格子ID得到格子列数
        /// </summary>
        /// <param name="cellId"><
        public static int getCellColumnByCellId(int cellId, int quadrant = 1)
        {
            var r1 = cellId % 11 == 0 ? 11 : (cellId % 11);
            var r2 = quadrant == 1 || quadrant == 4 ? r1 : (12 - r1) * -1;
            return r2;
        }
        /// <summary>
        /// 单纯在中间显示一些文字
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="time"></param>
        public static void showATips(string txt,float time)
        {
            var tm = TutorialManager.Instance;
            tm.showTipsAndContinue(txt, time,false);
        }
        /// <summary>
        /// 得到分数的字符串
        /// </summary>
        /// <param name="floats"></param>
        /// <returns></returns>
        public static string floatToFractionString(this float f)
        {
            var floats = f.floatToFraction();
            var numerator = floats[0];
            var denominator = floats[1];
            var theMax = Mathf.Max(numerator, denominator).ToString();
            string bs = "";
            for (int i = 0; i < theMax.Length; i++) {
                bs += "——";
            }
            return $"<align=center>{numerator}\n<size=50%>{bs}</size>\n{denominator}</align>";
        }
        public static string floatToFractionStringDirectly(float numerator,float denominator)
        {
            var theMax = Mathf.Max(numerator, denominator).ToString();
            string bs = "";
            for (int i = 0; i < theMax.Length; i++)
            {
                bs += "——";
            }
            return $"<align=center>{numerator}\n<size=50%>{bs}</size>\n{denominator}</align>";
        }
        /// <summary>
        /// 分数计算出小数float
        /// </summary>
        /// <param name="floats"></param>
        /// <returns></returns>
        public static float fractionStringToFloat(this float[] floats)
        {
            return floats[0]/floats[1];
        }

        /// <summary>
        /// 根据小数得到大致的分数数组 分子[0],分母[1](TMP富文本形式)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float[] floatToFraction(this float x)
        {
            if (float.IsNaN(x) || float.IsInfinity(x))
                return new float[] { 0, 1 };

            const int MaxDenominator = 9999999;
            const float Precision = 1e-6f; 
            const int MaxIterations = 100; 
            if (Mathf.Abs(x - Mathf.Floor(x)) < Precision)
                return new float[] { x, 1 };
            if (Mathf.Abs(x) < Precision)
                return new float[] { 0, 1 };

            bool isNegative = x < 0;
            x = Mathf.Abs(x);

            float a = x;
            int h_prev = 0, k_prev = 1;
            int h = 1, k = 0;
            int iterations = 0;
            int best_h = h, best_k = k;
            float best_error = float.MaxValue;

            while (true)
            {
                if (iterations++ > MaxIterations)
                    break;

                int ai = (int)Mathf.Floor(a);
                int temp_h = h;
                int temp_k = k;

                h = ai * h + h_prev;
                k = ai * k + k_prev;
                if (k > MaxDenominator)
                    break;
                float current_value = (float)h / k;
                float error = Mathf.Abs(x - current_value);
                if (error < best_error)
                {
                    best_error = error;
                    best_h = h;
                    best_k = k;
                }

                if (error < Precision)
                    break;

                h_prev = temp_h;
                k_prev = temp_k;

                float nextA = a - ai;
                if (nextA < Precision)
                    break;

                a = 1.0f / nextA;
            }
            if (best_k == 0)
                best_k = 1;

            int gcdValue = GCD(best_h, best_k);
            return new float[] { (isNegative ? -best_h : best_h) / (float)gcdValue, best_k / (float)gcdValue };
        }
        public static float[] simplifyFraction(float a, float b)
        {
            if (Mathf.Approximately(b, 0f))
            {
                Debug.LogError("分母不能为零");
                return null;
            }
            if (Mathf.Approximately(a, 0f))
            {
                return new float[] { 0f, 1f };
            }

            bool isNegative = (a < 0f) ^ (b < 0f);
            float absA = Mathf.Abs(a);
            float absB = Mathf.Abs(b);

            int gcdValue = GCD((int)absA, (int)absB);

            float simplifiedA = absA / gcdValue;
            float simplifiedB = absB / gcdValue;
            if (isNegative)
            {
                simplifiedA = -simplifiedA;
            }

            return new float[] { simplifiedA, simplifiedB };
        }
        /// <summary>
        /// 获取最大公约数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
        /// <summary>
        /// 根据两个端点生成线段
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Segment spawnASegment(PointEntity p1,PointEntity p2)
        {
            var segment = GameObject.Instantiate(GameManager.Instance.segmentPrefab);
            segment.p1 = p1;
            segment.p2 = p2;
            segment.enableSegment = true;
            Sounds.使用卡牌.playWithPitch();
            return segment;
        }
    }
}

