using UnityEngine;

/// <summary>
/// Métodos de extensión para <see cref="Rigidbody2D"/> que encapsulan
/// la lógica de movimiento avanzada del jugador (salto, jump-cut, física de salto).
/// </summary>
public static class MovementExtensions
{
    /// <summary>
    /// Aplica un salto instantáneo limpiando primero la velocidad Y actual
    /// para garantizar una altura de salto consistente independientemente del estado.
    /// </summary>
    /// <param name="rigidbody">Rigidbody2D objetivo.</param>
    /// <param name="jumpForce">Velocidad vertical a aplicar.</param>
    public static void Jump(this Rigidbody2D rigidbody, float jumpForce)
    {
        rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, jumpForce);
    }

    /// <summary>
    /// Recorta la altura del salto al 50 % de la velocidad Y actual.
    /// Se llama cuando el jugador suelta el botón de salto mientras asciende.
    /// </summary>
    /// <param name="rigidbody">Rigidbody2D objetivo.</param>
    public static void CutJump(this Rigidbody2D rigidbody)
    {
        rigidbody.linearVelocity = new Vector2(
            rigidbody.linearVelocity.x,
            rigidbody.linearVelocity.y * 0.5f
        );
    }

    /// <summary>
    /// Aplica la gravedad correcta según la fase del salto:
    /// ascenso normal, caída tras jump-cut o caída libre.
    /// <para>
    /// <b>Nota:</b> actualmente no se llama desde ningún estado porque cada estado
    /// llama a <c>Player.Gravity()</c> directamente. Se mantiene como utilidad.
    /// </para>
    /// </summary>
    /// <param name="rigidbody">Rigidbody2D objetivo.</param>
    /// <param name="isDashing">Si el jugador está dasheando (omitir cambios de gravedad).</param>
    /// <param name="jumpCutting">Si el salto fue cortado al soltar el botón.</param>
    public static void ApplyJumpPhysics(this Rigidbody2D rigidbody, bool isDashing, bool jumpCutting)
    {
        if (isDashing) return;

        if (jumpCutting)
            rigidbody.gravityScale = 7f;       // caída rápida tras cortar el salto
        else if (rigidbody.linearVelocity.y < 0f)
            rigidbody.gravityScale = 7.5f;     // caída libre normal
        else
            rigidbody.gravityScale = 3.5f;     // ascenso
    }
}