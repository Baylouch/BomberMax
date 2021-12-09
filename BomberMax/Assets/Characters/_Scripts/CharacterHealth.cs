using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterInfo))]
public class CharacterHealth : MonoBehaviour
{
    public const int maxHealthPoints = 2;

    public int healthPoints = 2;

    SpriteRenderer spriteRenderer;

    float invincibleAfterHit = 3f;
    float lastTimeHit = 0f;
    bool hasBeenHit = false;

    float endInvincibleBonusTime = 0f;
    bool invincibleBonus = false;
    Coroutine invincibleCoroutine;

    bool isDead = false;
    public bool IsDead()
    {
        return isDead;
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (hasBeenHit)
        {
            if (Time.time >= lastTimeHit + invincibleAfterHit)
            {
                hasBeenHit = false;
            }
        }

        if (invincibleBonus)
        {
            if (Time.time >= endInvincibleBonusTime)
            {
                invincibleBonus = false;
                StopCoroutine(invincibleCoroutine);
                spriteRenderer.color = new Color(1f, 1f, 1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Explosion")
        {
            if (hasBeenHit)
                return;

            if (invincibleBonus)
                return;

            lastTimeHit = Time.time;
            hasBeenHit = true;

            DecrementHealth();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator MultiColor()
    {
        bool _switchOnR = true;
        bool _switchOnG = false;
        bool _switchOnB = false;

        while (true)
        {
            if (_switchOnR)
            {
                while (spriteRenderer.color.r > .4f)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r - 0.1f, spriteRenderer.color.g + 0.1f, spriteRenderer.color.b, 1f);
                    yield return new WaitForSeconds(0.03f);

                }

                _switchOnR = false;
                _switchOnG = true;
            }
            else if (_switchOnG)
            {
                while (spriteRenderer.color.g > .4f)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g - 0.1f, spriteRenderer.color.b + 0.1f, 1f);
                    yield return new WaitForSeconds(0.03f);

                }

                _switchOnG = false;
                _switchOnB = true;
            }
            else if (_switchOnB)
            {
                while (spriteRenderer.color.b > .4f)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r + 0.1f, spriteRenderer.color.g, spriteRenderer.color.b - 0.1f, 1f);
                    yield return new WaitForSeconds(0.03f);

                }

                _switchOnB = false;
                _switchOnR = true;
            }
        }
    }

    IEnumerator GetHit()
    {
        bool _switch = false;

        while (hasBeenHit)
        {

            if (!_switch)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - 0.1f);

                if (spriteRenderer.color.a <= 0.2f)
                    _switch = true;
            }
            else
            {

                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + 0.1f);

                if (spriteRenderer.color.a >= 1f)
                    _switch = false;
            }

            yield return new WaitForSeconds(0.05f);
        }

        while (spriteRenderer.color.a < 1f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator Death()
    {
        Animator _anim = GetComponent<Animator>();

        isDead = true;

        GetComponent<Collider2D>().enabled = false;
        GetComponent<BombSpawner>().enabled = false;

        CharacterMovement _movement = GetComponent<CharacterMovement>();

        _movement.StopMovement();
        _movement.enabled = false;

        // Special condition for local player only
        Camera _playerCam = GetComponentInChildren<Camera>();
        if (_playerCam)
            _playerCam.transform.parent = null;

        spriteRenderer.sortingOrder = 5;

        _anim.SetTrigger("Death");

        float currentYPos = transform.position.y;

        

        while (transform.position.y < currentYPos + 1f)
        {
            transform.Translate(new Vector2(0f, 0.1f));

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(.5f);

        GetComponent<Rigidbody2D>().gravityScale = 1f;

        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);

        // TODO Change the way its use.
        //GameManager.instance.DisplayLoosePanel();
    }

    public void DecrementHealth()
    {
        healthPoints--;

        // TODO Change the way its use because this script is used on NOT local player and IA too.
        if (healthPoints == 1)
        {
            if (GetComponent<CharacterInfo>().IsLocal)
                GameCanvas.instance.healthUI.LostFirstHealth();

            StartCoroutine(GetHit());
        }
        else if (healthPoints <= 0)
        {
            if (GetComponent<CharacterInfo>().IsLocal)
                GameCanvas.instance.healthUI.LostSecondHealth();

            StartCoroutine(Death());
        }
    }

    public void IncrementHealth()
    {
        if (healthPoints < maxHealthPoints)
        {
            healthPoints++;

            if (GetComponent<CharacterInfo>().IsLocal)
                GameCanvas.instance.healthUI.GainHealth();
        }
    }

    public void SetupInvincibleBonus(float _duration)
    {
        // TO avoid to be multicolor indefinitly
        if (!invincibleBonus)
            invincibleCoroutine = StartCoroutine(MultiColor());

        endInvincibleBonusTime = Time.time + _duration;
        invincibleBonus = true;
   
    }


}
