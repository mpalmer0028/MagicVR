using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CircleBackgroundGenerator : MonoBehaviour
{
	public Font Font;
	public MagicMenu MagicMenu;
	
    public void MakeBackCircleImage()
	{
		
    	
		int width = 1080*2;
		int height = 1080*2;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Reset all pixels color to transparent
		Color32 resetColor = new Color32(0, 0, 0, 0);
		Color32 ringColor = new Color32(255, 255, 255, 200);
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
		DrawBridges(ref tex, MagicMenu, width/2f, width/4f, ringColor);
	    
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
		
	public static void DrawBridges(ref Texture2D texture, MagicMenu magicMenu, float rStart, float rEnd, Color color)
	{
		var sectionAngle = (float)(2 * Math.PI / magicMenu.Magics);
		//Debug.Log(rStart);
		//Debug.Log(rEnd);
		for (int m=0; m<magicMenu.Magics; m++) {
			var angle = sectionAngle * m;
				
			var x = (float)Math.Cos(angle);
			var y = (float)Math.Sin(angle);
			var center = new Vector2(texture.width/2,texture.height/2);
			
			var p1 = new Vector2((int)(x*rStart),(int)(y*rStart));
			var p2 = new Vector2((int)(x*rEnd),(int)(y*rEnd));
			
			for(var z=-2f;z<2f;z+=.01f){
				var q = Quaternion.Euler(0,0,z);
				DrawLine(ref texture, (Vector2)(q * p1) + center, (Vector2)(q * p2) + center, color);
			}
			
			
			
		}

	}		
	
	public static void DrawBridge(ref Texture2D texture, int x, int y, float rStart, float rEnd, float angle, Color color){
		
	}
	public static void DrawLine(ref Texture2D tex, Vector2 p1, Vector2 p2, Color col)
	{
		Vector2 t = p1;
		float frac = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
		float ctr = 0;
		
		while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) {
			t = Vector2.Lerp(p1, p2, ctr);
			ctr += frac;
			tex.SetPixel((int)t.x, (int)t.y, col);
		}
	}
}



















