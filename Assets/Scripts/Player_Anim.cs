using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Anim : MonoBehaviour
{
    [SerializeField] private Player player;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool("isMoving", player.IsMoving());
    }
}
