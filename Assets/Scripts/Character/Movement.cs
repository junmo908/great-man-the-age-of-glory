﻿using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
  public abstract class Movement : MonoBehaviour
  {
    protected new Rigidbody2D rigidbody;

    [Header("Movement")]
    [SerializeField]
    protected new Collider2D collider;

    protected bool canJump;

    [NonSerialized]
    public float curMoveDirection;

    public float moveSpeed = 1f;

    public float jumpPower = 3f;

    [NonSerialized]
    public bool canFlip = true;

    [NonSerialized]
    public Direction curDirection;

    public Vector2 curDirectionVector => curDirection switch
    {
      Direction.Left  => Vector2.left,
      Direction.None  => Vector2.zero,
      Direction.Right => Vector2.right,
      _               => Vector2.zero
    };

    [Header("Ground Check")]
    private float checkDistanceX;

    private float checkDistanceY;

    [FormerlySerializedAs("layerMask")]
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Animation Parameters")]
    [SerializeField]
    private string walkingAnim;

    [SerializeField]
    private string jumpingAnim;

    protected Animator animator;

    protected virtual void Awake()
    {
      rigidbody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();

      rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

      var bounds = collider.bounds;
      checkDistanceX = bounds.extents.x;
      checkDistanceY = bounds.extents.y + 0.1f;
    }

    protected virtual void FixedUpdate()
    {
      transform.Translate(curMoveDirection * moveSpeed * Time.fixedDeltaTime, 0f, 0f);
    }

    protected virtual void Update()
    {
      if (!string.IsNullOrEmpty(walkingAnim)) 
        animator.SetBool(walkingAnim, Mathf.Abs(curMoveDirection) > 0);
      CheckGround();
      Flip();
    }

    protected virtual void Jump()
    {
      if (!canJump)
        return;

      SetJump(true);
      rigidbody.velocity = Vector2.up * jumpPower;
    }

    protected void Move(float amount) => curMoveDirection = amount;

    protected void Move(Direction dir) => curMoveDirection = (float)dir;

    protected void CheckGround()
    {
      const float distance = 0.3f;
      var pos = GetCenterPosition();
      var leftVector = new Vector2(pos.x - checkDistanceX, pos.y - checkDistanceY);
      var rightVector = new Vector2(pos.x + checkDistanceX, pos.y - checkDistanceY);

      var hitLeft = Physics2D.Raycast(leftVector, Vector2.down, distance, groundLayer);
      var hitRight = Physics2D.Raycast(rightVector, Vector2.down, distance, groundLayer);

      Debug.DrawRay(leftVector, Vector3.down * distance, Color.green);
      Debug.DrawRay(rightVector, Vector3.down * distance, Color.green);

      var check = (hitLeft || hitRight);
      canJump = check;
      SetJump(!check);
    }

    protected void SetJump(bool value)
    {
      if (string.IsNullOrEmpty(jumpingAnim)) return;
      animator.SetBool(jumpingAnim, value);
    }

    protected void Flip()
    {
      if (!canFlip)
        return;

      var scale = transform.localScale;
      scale.x = curMoveDirection switch
      {
        > 0 => Mathf.Abs(scale.x),
        < 0 => -Mathf.Abs(scale.x),
        _ => scale.x
      };
      transform.localScale = scale;

      curDirection = scale.x > 0 ? Direction.Right : Direction.Left;
    }

    protected Vector2 GetCenterPosition()
    {
      var position = (Vector2)transform.position;
      var offset = collider.offset;

      return position + offset;
    }
    
  }
}
