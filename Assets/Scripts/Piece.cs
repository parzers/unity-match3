using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Sprite[] sprites;
    public float _moveSpeed = 5f;

    private int _row;
    private int _column;
    private int _type;

    private SpriteRenderer _sr;
    private Board _board;

    public GameObject selector;

    public void SetBoard(Board board)
    {
        _board = board;
    }

    public void Select()
    {
        selector.SetActive(true);
        selector.GetComponent<Animator>().Play("Idle");
    }

    public void Unselect()
    {
        selector.SetActive(false);
    }

    new public int GetType()
    {
        return _type;
    } 
    public void SetType(int type)
    {
        _type = type;
        _sr.sprite = sprites[type];
    }


    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        Unselect();
        SetType(Random.Range(0, 7));
    }

    private void Update()
    {

    }

    public int GetRow()
    {
        return _row;
    }

    public int GetColumn()
    {
        return _column;
    }

    public void SetPosition(int row, int column)
    {

        transform.localPosition = new Vector3(column, row);
        _row = row;
        _column = column;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        _board.SelectPiece(this);
    }
}
