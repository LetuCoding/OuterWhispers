using System.Collections;
using UnityEngine;



    public static class MovementExtensions
    {
        //=====================================================================================================
        //                             Extension methods for movement logic
        //=====================================================================================================
        
        public static void Jump(this Rigidbody2D rigidbody, float jumpForce)
        {
            
            // Limpiamos la velocidad Y actual para que el salto sea consistente
            
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, 0); 
    
            
            
            
            // Aplicamos la fuerza instantánea cambiando la velocidad directamente.
            // Al haber subido la gravedad en el editor, necesitas subir jumpForce.
            
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, jumpForce);
            
            
        }
        
        
        public static void CutJump(this Rigidbody2D rigidbody)
        {
            
            
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x,rigidbody.linearVelocity.y * 0.5f);
            
            
        }

        
        
        public static void ApplyJumpPhysics(this Rigidbody2D rigidbody, bool isDashing, bool jumpCutting)
        {
            if (isDashing) return;
            
            if (jumpCutting)
            {
                rigidbody.gravityScale = 7f;  // caída fuerte tras soltar
            }
            else if (rigidbody.linearVelocity.y < 0)
            {
                rigidbody.gravityScale = 7.5f;  // caída rápida normal
            }
            else
            {
                rigidbody.gravityScale = 3.5f; // ascenso
            
            }
        }
        
        
     
        
        
        
        
    }
