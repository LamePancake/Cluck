// original by Mr. Animator
// adapted to C# by @torahhorse
// http://wiki.unity3d.com/index.php/Headbobber

using System.Collections;
using System;
using Microsoft.Xna.Framework;

public class HeadBob
{	
	private float bobbingSpeed;
    public float bobbingAmount; 
	public float  midpoint; 
	
	private float timer = 0.0f; 
 
	public Vector3 Update (float xDelta, float yDelta, Vector3 position, float time, float speed, float amount)
	{ 
	    float waveslice = 0.0f;
        float horizontal = xDelta;
        float vertical = yDelta;
        bobbingSpeed = speed;
        bobbingAmount = amount;
        midpoint = position.Y;
	    
	    if (Math.Abs(horizontal) == 0f && Math.Abs(vertical) == 0f)
	    { 
	       timer = 0.0f; 
	    } 
	    else
	    { 
	       waveslice = (float)Math.Sin(timer); 
	       timer = timer + bobbingSpeed; 
	       if (timer > Math.PI * 2f)
	       { 
	          timer = timer - (float)(Math.PI * 2.0f); 
	       } 
	    } 
	    if (waveslice != 0f)
	    { 
	       float translateChange = waveslice * bobbingAmount; 
	       float totalAxes = Math.Abs(horizontal) + Math.Abs(vertical); 
	       totalAxes = MathHelper.Clamp (totalAxes, 0.0f, 1.0f); 
	       translateChange = totalAxes * translateChange;

           Vector3 localPos = position;
           localPos.Y = midpoint + translateChange * time;
           position = localPos;
           return position;
	    } 
	    else
	    {
            Vector3 localPos = position;
	    	localPos.Y = midpoint;
            position = localPos;
            return position;
	    } 
	}
}
