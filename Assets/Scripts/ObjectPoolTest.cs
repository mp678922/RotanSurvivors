using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolTest : MonoBehaviour {
    public Player redTarget;
    public Player blackTarget;
    private Pool<Player> _poolRed;
    private Pool<Player> _poolBlack;
    public void Start() {
        _poolRed = new Pool<Player>(redTarget, 100);
        _poolBlack = new Pool<Player>(blackTarget, 20);
    }
    public void CreateRedObject() {
        Player controller = _poolRed.Create();
        StartCoroutine(ReturnRedObjectAsync(controller));
    }

    public void CreateBlackObject() {
        Player controller = _poolBlack.Create();
        StartCoroutine(ReturnObjectBlackAsync(controller));
    }

    IEnumerator ReturnRedObjectAsync(Player returnObject) {
        yield return new WaitForSeconds(5f);
        _poolRed.Return(returnObject);
    }
    IEnumerator ReturnObjectBlackAsync(Player returnObject) {
        yield return new WaitForSeconds(5f);
        _poolBlack.Return(returnObject);
    }

}
