using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class EnemySpawner : MonoBehaviour {

    /*
     * 
     * 
     *      ENEMYSPAWNERS USE INHERITANCE AND INHERIT FROM THIS CLASS
     * 
     * 
     */
    public Transform spawnPrefab;
    public int desiredEnemyCount;           //will spawn new enemies while enemyCount < desiredEnemyCount
    [HideInInspector]
    public int enemyCount;
    //public List<GameObject> enemyList;          //enemies from this spawner will be in this list
                                            //(enemies manually placed in here in the editor are not technically spawned from this spawner, but are treated as if they had)

    public float spawnDelay;                //spawn enemy after this delay;
    private State state;
    public enum State
    {
        Inactive, Active
    }
    LevelState stateData;
    private BoxCollider2D triggerZone;
    public bool activateOnPlayerEnter = false;          //if Inactive, become active
    private SpriteRenderer spriteRenderer;
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    // Use this for initialization
    void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        triggerZone = GetComponent<BoxCollider2D>();
        if (triggerZone == null)
            activateOnPlayerEnter = false;
        stateData = GetComponent<LevelState>();
        stateData.initialLoad();
        state = (State)stateData.getState();
        setActive((int)state);
	}
    private void Update()
    {
        if (enemyCount < desiredEnemyCount)
        {
            StartCoroutine(spawnAfterDelay(spawnDelay));
            ++enemyCount;
        }
    }

    public abstract void spawnEnemy();          //the main difference between spawners is that different enemy types need different starting data

    protected IEnumerator spawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        spawnEnemy();
    }
    public bool active()
    {
        return state == State.Active;
    }
    public void setActive(int x)    //0 is inactive, 1 is active
    {
        state = (State)x;
        switch (state)
        {
            case (State.Active):
                spriteRenderer.sprite = activeSprite;
                break;
            case (State.Inactive):
                spriteRenderer.sprite = inactiveSprite;
                break;
        }
    }
    public void removeEnemy()
    {
        --enemyCount;
    }
}
