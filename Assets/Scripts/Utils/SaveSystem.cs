using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class SaveSystem
{
    // 保存先パス
    private static readonly string SAVE_PATH = Application.persistentDataPath + "/savedata/";
    private static readonly string PLAYER_DATA_FILE = "player_data.json";
    private static readonly string CUSTOM_RECIPES_FOLDER = "custom_recipes/";
    
    // プレイヤーデータを保存するメソッド
    public static void SavePlayerData(PlayerData data)
    {
        try
        {
            // ディレクトリが存在することを確認
            EnsureDirectoryExists(SAVE_PATH);
            
            // データをJSONに変換
            string json = JsonUtility.ToJson(data, true);
            
            // ファイルに書き込み
            File.WriteAllText(SAVE_PATH + PLAYER_DATA_FILE, json);
            
            Debug.Log("Player data saved to: " + SAVE_PATH + PLAYER_DATA_FILE);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving player data: " + e.Message);
        }
    }
    
    // プレイヤーデータを読み込むメソッド
    public static PlayerData LoadPlayerData()
    {
        string filePath = SAVE_PATH + PLAYER_DATA_FILE;
        
        if (File.Exists(filePath))
        {
            try
            {
                // ファイルからJSONを読み込み
                string json = File.ReadAllText(filePath);
                
                // JSONをデータに変換
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                
                Debug.Log("Player data loaded from: " + filePath);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading player data: " + e.Message);
            }
        }
        else
        {
            Debug.Log("No player data file found at: " + filePath);
        }
        
        return null;
    }
    
    // カスタムレシピを保存するメソッド
    public static void SaveCustomRecipe(CustomRecipeData recipe)
    {
        try
        {
            // ディレクトリが存在することを確認
            string recipesPath = SAVE_PATH + CUSTOM_RECIPES_FOLDER;
            EnsureDirectoryExists(recipesPath);
            
            // データをJSONに変換
            string json = JsonUtility.ToJson(recipe, true);
            
            // ファイル名の作成
            string fileName = recipe.id + ".json";
            
            // ファイルに書き込み
            File.WriteAllText(recipesPath + fileName, json);
            
            Debug.Log("Custom recipe saved to: " + recipesPath + fileName);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving custom recipe: " + e.Message);
        }
    }
    
    // カスタムレシピをすべて読み込むメソッド
    public static List<CustomRecipeData> LoadCustomRecipes()
    {
        List<CustomRecipeData> recipes = new List<CustomRecipeData>();
        string recipesPath = SAVE_PATH + CUSTOM_RECIPES_FOLDER;
        
        if (Directory.Exists(recipesPath))
        {
            try
            {
                // ディレクトリ内のすべてのJSONファイルを取得
                string[] files = Directory.GetFiles(recipesPath, "*.json");
                
                foreach (string file in files)
                {
                    try
                    {
                        // ファイルからJSONを読み込み
                        string json = File.ReadAllText(file);
                        
                        // JSONをデータに変換
                        CustomRecipeData recipe = JsonUtility.FromJson<CustomRecipeData>(json);
                        
                        if (recipe != null)
                        {
                            recipes.Add(recipe);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error loading custom recipe from {file}: {e.Message}");
                    }
                }
                
                Debug.Log($"Loaded {recipes.Count} custom recipes from: {recipesPath}");
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading custom recipes: " + e.Message);
            }
        }
        else
        {
            Debug.Log("No custom recipes directory found at: " + recipesPath);
        }
        
        return recipes;
    }
    
    // セーブデータを削除するメソッド
    public static void DeleteSaveData()
    {
        try
        {
            // プレイヤーデータファイルを削除
            string filePath = SAVE_PATH + PLAYER_DATA_FILE;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Player data file deleted: " + filePath);
            }
            
            // カスタムレシピディレクトリを削除
            string recipesPath = SAVE_PATH + CUSTOM_RECIPES_FOLDER;
            if (Directory.Exists(recipesPath))
            {
                Directory.Delete(recipesPath, true);
                Debug.Log("Custom recipes directory deleted: " + recipesPath);
            }
            
            Debug.Log("All save data deleted successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error deleting save data: " + e.Message);
        }
    }
    
    // セーブデータが存在するかチェックするメソッド
    public static bool HasSaveData()
    {
        string filePath = SAVE_PATH + PLAYER_DATA_FILE;
        return File.Exists(filePath);
    }
    
    // ディレクトリが存在することを確認するヘルパーメソッド
    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
