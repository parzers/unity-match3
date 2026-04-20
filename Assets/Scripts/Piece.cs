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

    private SpriteRenderer _sr;
    private Board _board;

    public enum State
    {
        Idle,
        Selected,
        SwapAnimation,
        MoveAnimation,
        DestroyAnimation,
    }
    public State _state;

    public GameObject selector;

    public void SetBoard(Board board)
    {
        _board = board;
    }

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
        if (_state == State.MoveAnimation || _state == State.SwapAnimation)
        {
            if (_board._animationsEnabled)
            {
                transform.localPosition = Vector2.MoveTowards(
                    transform.localPosition, _animationTargetPosition, Time.deltaTime * _moveSpeed);

                if (transform.localPosition == _animationTargetPosition)
                {
                    _state = State.Idle;
                }
            } 
            else
            {
                _state = State.Idle;
                transform.localPosition = _animationTargetPosition;
            }
        }
        if (_state == State.DestroyAnimation)
        {
            if (_board._animationsEnabled)
            {
                Animator animator = GetComponent<Animator>();
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("Destroy") && state.normalizedTime >= 1.0f)
                {
                    _board.RemovePiece(_row, _column);
                    Destroy(gameObject);
                    _state = State.Idle;
                }
            }
            else
            {
                _board.RemovePiece(_row, _column);
                Destroy(gameObject);
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

    public void SetPosition(int row, int column)
    {

        transform.localPosition = new Vector3(column, row);
        _row = row;
        _column = column;
    }

    public void MovePosition(int row, int column)
    {
        _animationTargetPosition = new Vector3(column, row);
        _state = State.MoveAnimation;
        _row = row;
        _column = column;
    }

    public void SwapPosition(int row, int column)
    {
        _animationTargetPosition = new Vector3(column, row);
        _state = State.SwapAnimation;
        _row = row;
        _column = column;
    }

    public void Destroy()
    {
        _state = State.DestroyAnimation;
        GetComponent<Animator>().SetBool("Destroyed", true);
    }

    public bool IsAlive()
    {
        return _state != State.DestroyAnimation;
    }

    private void OnMouseDown()
    {
        if (_state == State.Idle && _board.GetState() == Board.State.Idle)
        {
            _board.SelectPiece(this);
        }
    }
}
