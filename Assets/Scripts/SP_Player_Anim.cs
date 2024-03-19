using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SP_Player_Anim : MonoBehaviour
{
    [SerializeField] private SP_Player player;
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
