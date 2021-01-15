using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CircleBackgroundGenerator : MonoBehaviour
{
	public Font Font;
	
    public void MakeBackCircleImage()
	{
		
    	
		int width = 1080*2;
		int height = 1080*2;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Reset all pixels color to transparent
        Color32 resetColor = new Color32(255, 255, 255, 0);
		Color32 ringColor = new Color32(50, 255, 50, 200);
        Color32[] resetColorArray = tex.GetPixels32();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

		tex.SetPixels32(resetColorArray);
		//outer
		DrawCircle(ref tex,width/2,height/2, width/2f, ringColor);
		DrawCircle(ref tex,width/2,height/2, width/2.1f, resetColor);
	    
		DrawCircle(ref tex,width/2,height/2, width/4f, ringColor);
		DrawCircle(ref tex,width/2,height/2, width/4.5f, resetColor);
	    
		//var ttt = new TextToTexture(Font.);
	    
        tex.Apply();

        System.IO.File.WriteAllBytes(Application.dataPath + "/GeneratedImages/CircleBackground.png", tex.EncodeToPNG());
		AssetDatabase.Refresh();
    }
    
	public static void DrawCircle(ref Texture2D texture, int x, int y, float r, Color color)
	{
		float rSquared = r * r;
		for (int u=0; u<texture.width; u++) {
			for (int v=0; v<texture.height; v++) {
				if ((x-u)*(x-u) + (y-v)*(y-v) < rSquared) texture.SetPixel(u,v,color);
			}
		}

	}
	
	

}
