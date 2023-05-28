using UnityEngine;

public class TileClickDetector : MonoBehaviour
{
    private TileProperties tileProperties;
    private PawnMover pawnMover;

    private void Awake()
    {
        tileProperties = GetComponent<TileProperties>();
    }

    private void Start()
    {
        pawnMover = GetComponentInParent<PawnMover>();
    }

    public void ChildPawnClicked()
    {
        MouseDown();
    }

    public void MouseDown()
    {
        if (tileProperties.IsOccupied())
            pawnMover.PawnClicked(tileProperties.GetPawn());
        else
            pawnMover.TileClicked(this.gameObject);
    }

    public void ManualTileClick() {
        pawnMover.TileClicked(this.gameObject);
    }

    public void ManualPawnClick() {
        pawnMover.PawnClicked(tileProperties.GetPawn());
    }

    public void ClickTile()
    {
        MouseDown();
    }
}