using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ExplosionAnimation : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(SimpleAnimation());
    }

    // This coroutine simply switch "flipY" value on the spriterenderer to animate the explosion
    IEnumerator SimpleAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            spriteRenderer.flipY = !spriteRenderer.flipY;

        }
    }
}
