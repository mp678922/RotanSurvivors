using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Level : MonoBehaviour {
    public Unit enemyOrigin;
    public List<Unit> enemy = new List<Unit>();
    public int maxEnemy = 5;
    public Pool<Unit> enemyPool;

    void Start() {
        enemyPool = new Pool<Unit>(enemyOrigin);
        StartCoroutine(Loop());
    }
    IEnumerator Loop() {
        while (true) {
            Turn();
            yield return new WaitForSeconds(1f);
        }
    }

    void Turn() {
        if (enemy.Count < maxEnemy) {
            Unit newEnemy = enemyPool.Create();
            newEnemy.onUnitDead = OnEnemyDead;
            enemy.Add(newEnemy);
            Rect screen = GetScreen();
            float randomX = Random.Range(screen.x, screen.x + screen.width);
            float randomY = Random.Range(screen.y, screen.y + screen.height);
            newEnemy.transform.position = new Vector3(randomX, randomY, newEnemy.transform.position.z);
        }
    }
    public void OnEnemyDead(Unit who) {
        enemy.Remove(who);
        enemyPool.Return(who);
    }

    Rect GetScreen() {
        Vector2 camPos = Camera.main.transform.position;
        float width = Camera.main.orthographicSize * (Screen.width / Screen.height);
        float x = camPos.x - width;
        float y = camPos.y - Camera.main.orthographicSize;
        return new Rect(x, y, width * 2f, Camera.main.orthographicSize * 2f);
    }

}
