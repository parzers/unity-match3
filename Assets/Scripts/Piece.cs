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

    private Vector3 _animationTargetPosition;

    SpriteRenderer sr;


    private Board board;

    public enum State
    {
        Idle,
        Selected,
        MoveAnimation,
    }
    public State _state;

    public GameObject selector;

    public void Select()
    {
        _state = State.Selected;
        selector.SetActive(true);
        selector.GetComponent<Animator>().Play("Idle");
    }

    public void Unselect()
    {
        _state = State.Idle;
        selector.SetActive(false);
    }

    public Board Board
    {
        get { return board; }
        set { board = value; }
    }

    private void ResetTransformPosition()
    {
        transform.localPosition = new Vector2(_column, _row);
    }

    new public int GetType()
    {
        return _type;
    } 
    public void SetType(int type)
    {
        _type = type;
        sr.sprite = sprites[type];
    }


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Unselect();
        SetType(Random.Range(0, 7));
    }

    private void Update()
    {
        if (_state == State.MoveAnimation)
        {
            transform.localPosition = Vector2.MoveTowards(
                transform.localPosition, _animationTargetPosition, Time.deltaTime * _moveSpeed);

            if (transform.localPosition == _animationTargetPosition)
            {
                _state = State.Idle;
            }
        }
    }

    public int GetRow()
    {
        return _row;
    }

    public int GetColumn()
    {
        return _column;
    }

    public void SetPosition(int row, int column, bool animate)
    {
        if (animate)
        {
            _animationTargetPosition = new Vector2(column, row);
            _state = State.MoveAnimation;
        }
        else
        {
            transform.localPosition = new Vector3(column, row);
        }
        _row = row;
        _column = column;
    }

    private void OnMouseDown()
    {
        if (_state == State.Idle)
        {
            board.SelectPiece(this);
        }
    }
}
