// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 19/03/2022 @ 13:02
// ==========================================================================

using UnityEngine;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    public interface IEnemy
    {
        float MoveSpeed { get; }
        
        Vector2 Position { get; }

        EnemySurroundings GetSurroundings();
        
        void StandStill();

        void Jump();

        void Move(float horizontalMovement);

        bool IsBlocked(float horizontalMovement);
    }
}