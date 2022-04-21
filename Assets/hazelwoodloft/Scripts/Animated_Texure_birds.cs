using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Animated_Texure_birds : MonoBehaviour
{
    public int uvAnimationTileX; //Here you can place the number of columns of your sheet. 
     //The above sheet has 24
    public int uvAnimationTileY; //Here you can place the number of rows of your sheet. 
     //The above sheet has 1
    public float framesPerSecond;
    public virtual void Update()
    {
        // Calculate index
        int index = (int) (Time.time * this.framesPerSecond);
        // repeat when exhausting all frames
        index = index % (this.uvAnimationTileX * this.uvAnimationTileY);
        // Size of every tile
        Vector2 size = new Vector2(1f / this.uvAnimationTileX, 1f / this.uvAnimationTileY);
        // split into horizontal and vertical index
        int uIndex = index % this.uvAnimationTileX;
        int vIndex = index / this.uvAnimationTileX;
        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        Vector2 offset = new Vector2(uIndex * size.x, (1f - size.y) - (vIndex * size.y));
        this.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        this.GetComponent<Renderer>().material.SetTextureScale("_MainTex", size);
    }

    public Animated_Texure_birds()
    {
        this.uvAnimationTileX = 6;
        this.uvAnimationTileY = 1;
        this.framesPerSecond = 1f;
    }

}