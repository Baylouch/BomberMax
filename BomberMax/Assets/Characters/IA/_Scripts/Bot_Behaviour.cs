/* Bot_behaviour.cs
 * 
 * Bot get infos such as bombs in game, game bonus, enemy positions and will react to this relatively to its difficulty and the distance of them.
 * 
 * It retain the bomber ID and its explosion force when a bomb explode.
 * 
 * A path is created to set its movement when it goes for chase player, get bonus...
 * 
 * It has a list of danger positions and a method is used to create a path to a safe position.
 * 
 * When it's stuck because of undestructible block, border limit and danger position, it's stop and wait until the bomb has explode.
 * 
 * */

using System.Collections.Generic;
using UnityEngine;

// To know bot priority, more the value is more the priority is. None is when there is nothing around the bot he just walk to another place

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(BombSpawner))]
[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterInfo))]
public class Bot_Behaviour : MonoBehaviour
{
    CharacterHealth health;
    CharacterMovement movement;
    BombSpawner bombSpawner;
    CharacterInfo infos;

    float AIVision = 3f;

    Dictionary<int, int> bombLengthByID = new Dictionary<int, int>(); // Dictionnary to match key (character ID) with value (known bomb length)

    // TODO Make a logic to not go on our step until we must to
    [SerializeField]List<Vector2> path;
    [SerializeField]List<Vector2> nextPath; // To use as safe path (TODO change the name or the way to use)

    [SerializeField] int pathIndex = 0;
    bool hasPath;
    bool reachedEndPath = false;

    bool isInDanger = false; // to know if the bot is on a danger position

    MovementDirection movementDirection;

    bool dropBomb = false; // To know when bot needs to drop a bomb

    float timeBeforeTryNewPath = 1f;
    float pathTimerStart = 0f;

    float timeBetweenBomb = 10f; // To wait until the bot drop another bomb
    float lastTimeBombDropped = -10f;
    Vector2 dropPosition;

    bool temp = false;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<CharacterHealth>();
        movement = GetComponent<CharacterMovement>();
        bombSpawner = GetComponent<BombSpawner>();
        infos = GetComponent<CharacterInfo>();     
    }

    // Update is called once per frame
    void Update()
    {
        if (health.IsDead())
            return;


    }

   

    void SetInDanger()
    {
        Vector2 _currentPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

        int _tileIndex = StageManager.instance.Grid.FindIndex(x => x.position == _currentPosition);

        if (_tileIndex != -1 && StageManager.instance.Grid[_tileIndex].isDanger)
        {
            isInDanger = true;
            return;
        }

        isInDanger = false;
    }

    // Method used in Bomb.cs Explode() method
    public void UpdateBombsExplosionDictionary(BombSpawner _spawner)
    {
        if (infos.CharID == _spawner.GetCharID()) // We already know our explosion force
            return;

        // We try to get the bomb ID in the dictionnary to set the new length we know
        bool _containsID = bombLengthByID.ContainsKey(_spawner.GetCharID());

        if (_containsID)
        {
            bombLengthByID[_spawner.GetCharID()] = _spawner.GetExplosionForce();
        }
        else
        {
            bombLengthByID.Add(_spawner.GetCharID(), _spawner.GetExplosionForce());
        }
    }
}
