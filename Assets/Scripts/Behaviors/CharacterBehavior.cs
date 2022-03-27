// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 19:34
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using NoSuchCompany.Games.SuperMario.Entities;
using NoSuchCompany.Games.SuperMario.Services;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Behaviors
{
    /// <summary>
    /// Represents a character that can move around
    /// the scene and interacts with the foreground.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class CharacterBehavior : MonoBehaviour
    {
        //  Private fields.
        private readonly IRaycaster _raycaster;
        
        //  Unity injected properties.
        public LayerMask collisionMask;
        
        //  Public properties. 
        public ICollisions Collisions => _raycaster.Collisions;

        public CharacterBehavior()
        {
            _raycaster = new Raycaster();
        }
        
        public void Start()
        {
            var boxCollider2D = GetComponent<BoxCollider2D>();
            
            _raycaster.Initialize(boxCollider2D, 6);
        }

        public void Move(Vector3 objectVelocity)
        {
            _raycaster.ApplyCollisions(ref objectVelocity, collisionMask);
            
            transform.Translate(objectVelocity);
        }

        public IEnumerable<IRaycastCollision> FindVerticalHits(ref Vector3 characterVelocity)
        {
            return _raycaster.FindVerticalHitsOnly(characterVelocity, collisionMask).ToList();
        }

        public void Kill()
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.Deads);
            Destroy (gameObject, (int)TimeSpan.FromSeconds(2).TotalSeconds);
        }
    }
}