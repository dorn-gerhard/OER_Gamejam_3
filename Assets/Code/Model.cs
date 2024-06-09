using R3;

public class Model
{
    public int health;
    public float horizontalInput;
    public float horizontalSpeed;
    public bool isJumping;
    public float minJumpHeight;
    public float maxJumpHeight;
    public float jumpSpeed;
    public ReactiveProperty<bool> isDropping;
    public float platformDeactivationDuration;
    public float dropSpeed;

    public Model()
    {
        health = 5;
        minJumpHeight = 2;
        maxJumpHeight = 8;
        horizontalSpeed = 12;
        platformDeactivationDuration = 0.5f;
        isDropping = new();
        dropSpeed = 25F;
        jumpSpeed = 25F;
    }
}
