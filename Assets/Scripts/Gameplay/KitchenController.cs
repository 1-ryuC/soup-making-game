using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoupMaking.Gameplay
{
    /// <summary>
    /// キッチン環境全体を管理するコントローラークラス
    /// </summary>
    public class KitchenController : MonoBehaviour
    {
        [Header("キッチンエリア")]
        [SerializeField] private Transform cookingArea;
        [SerializeField] private Transform ingredientStorageArea;
        [SerializeField] private Transform tastingArea;

        [Header("カメラ設定")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3 cookingAreaCameraPosition;
        [SerializeField] private Vector3 ingredientStorageCameraPosition;
        [SerializeField] private Vector3 tastingAreaCameraPosition;
        [SerializeField] private float cameraMoveSpeed = 2.0f;

        [Header("トランジション設定")]
        [SerializeField] private float transitionDuration = 1.0f;
        [SerializeField] private AnimationCurve transitionCurve;

        // アニメーション用変数
        private Vector3 cameraStartPosition;
        private Vector3 cameraTargetPosition;
        private float transitionTime;
        private bool isTransitioning;

        // 現在のエリア
        private KitchenArea currentArea = KitchenArea.CookingArea;

        // イベント
        public event Action<KitchenArea> OnAreaChanged;

        private void Start()
        {
            // メインカメラの確認
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.LogError("Main camera not found.");
                }
            }

            // キッチンエリアの確認
            ValidateKitchenAreas();

            // 初期エリアを設定
            SetCurrentArea(KitchenArea.CookingArea, true);
        }

        private void Update()
        {
            // カメラの移動アニメーション
            if (isTransitioning)
            {
                transitionTime += Time.deltaTime;
                float t = transitionCurve.Evaluate(Mathf.Clamp01(transitionTime / transitionDuration));
                
                mainCamera.transform.position = Vector3.Lerp(cameraStartPosition, cameraTargetPosition, t);
                
                if (transitionTime >= transitionDuration)
                {
                    isTransitioning = false;
                }
            }
        }

        /// <summary>
        /// キッチンエリアの参照が正しいか確認
        /// </summary>
        private void ValidateKitchenAreas()
        {
            if (cookingArea == null)
            {
                Debug.LogError("Cooking area is not assigned.");
            }
            
            if (ingredientStorageArea == null)
            {
                Debug.LogError("Ingredient storage area is not assigned.");
            }
            
            if (tastingArea == null)
            {
                Debug.LogError("Tasting area is not assigned.");
            }
        }

        /// <summary>
        /// 現在のエリアを設定し、カメラを移動
        /// </summary>
        /// <param name="area">移動先のエリア</param>
        /// <param name="immediate">即座に移動するかどうか</param>
        public void SetCurrentArea(KitchenArea area, bool immediate = false)
        {
            if (area == currentArea && !immediate) return;
            
            currentArea = area;
            
            // カメラの目標位置を設定
            Vector3 targetPosition = Vector3.zero;
            switch (area)
            {
                case KitchenArea.CookingArea:
                    targetPosition = cookingAreaCameraPosition;
                    break;
                
                case KitchenArea.IngredientStorage:
                    targetPosition = ingredientStorageCameraPosition;
                    break;
                
                case KitchenArea.TastingArea:
                    targetPosition = tastingAreaCameraPosition;
                    break;
            }
            
            // カメラ移動開始
            if (immediate)
            {
                // 即座に移動
                mainCamera.transform.position = targetPosition;
            }
            else
            {
                // トランジション開始
                StartCameraTransition(targetPosition);
            }
            
            // イベント発火
            OnAreaChanged?.Invoke(currentArea);
            Debug.Log($"Kitchen area changed to: {currentArea}");
        }

        /// <summary>
        /// カメラトランジションを開始
        /// </summary>
        private void StartCameraTransition(Vector3 targetPosition)
        {
            isTransitioning = true;
            transitionTime = 0f;
            cameraStartPosition = mainCamera.transform.position;
            cameraTargetPosition = targetPosition;
        }

        /// <summary>
        /// 食材保管エリアを開く
        /// </summary>
        public void OpenIngredientStorage()
        {
            SetCurrentArea(KitchenArea.IngredientStorage);
        }

        /// <summary>
        /// 食材保管エリアを閉じる（調理エリアに戻る）
        /// </summary>
        public void CloseIngredientStorage()
        {
            SetCurrentArea(KitchenArea.CookingArea);
        }

        /// <summary>
        /// 試食エリアに移動
        /// </summary>
        public void GoToTastingArea()
        {
            SetCurrentArea(KitchenArea.TastingArea);
        }

        /// <summary>
        /// 調理エリアに戻る
        /// </summary>
        public void ReturnToCookingArea()
        {
            SetCurrentArea(KitchenArea.CookingArea);
        }
    }

    /// <summary>
    /// キッチンのエリアを表す列挙型
    /// </summary>
    public enum KitchenArea
    {
        CookingArea,         // 調理エリア
        IngredientStorage,   // 食材保管エリア
        TastingArea          // 試食エリア
    }
}