using UnityEngine;

public class Piece : MonoBehaviour
{
	public bool isWhite;
	public bool isKing;

	public bool IsForceToMove(Piece[,] board, int x, int y)
	{
		if (isWhite || isKing)
		{
			// Góra lewo
			if (x >= 2 && y <= 5)
			{
				Piece p = board[x - 1, y + 1];
				// Jesli pion nie jest tego samego koloru to zbij
				if (p != null && p.isWhite != isWhite)
				{
					// Spradzenie mozliwosci na wyladowanie po ruchu
					if (board[x - 2, y + 2] == null)
						return true;
				}
			}

			// Góra prawo
			if (x <= 5 && y <= 5)
			{
				Piece p = board[x + 1, y + 1];
				// Jesli pion nie jest tego samego koloru to zbij
				if (p != null && p.isWhite != isWhite)
				{
					// Spradzenie mozliwosci na wyladowanie po ruchu
					if (board[x + 2, y + 2] == null)
						return true;
				}
			}
		}

		if (!isWhite || isKing)
		{
			// Dół lewo
			if (x >= 2 && y >= 2)
			{
				Piece p = board[x - 1, y - 1];
				// Jesli pion nie jest tego samego koloru to zbij
				if (p != null && p.isWhite != isWhite)
				{
					// Spradzenie mozliwosci na wyladowanie po ruchu
					if (board[x - 2, y - 2] == null)
						return true;
				}
			}

			// Dół prawo
			if (x <= 5 && y >= 2)
			{
				Piece p = board[x + 1, y - 1];
				// Jesli pion nie jest tego samego koloru to zbij
				if (p != null && p.isWhite != isWhite)
				{
					// Spradzenie mozliwosci na wyladowanie po ruchu
					if (board[x + 2, y - 2] == null)
						return true;
				}
			}
		}

		return false;
	}

	public bool ValidMove(Piece[,] board, int startX, int startY, int endX, int endY)
	{
		// Przejscie nad pionem
		if (board[endX, endY] != null)
			return false;

		int deltaMoveX = Mathf.Abs(startX - endX);
		int deltaMoveY = endY - startY;

		if (isWhite || isKing)
		{
			if (deltaMoveX == 1) // zwykly ruch
			{
				if (deltaMoveY == 1)
					return true;
			}
			else if (deltaMoveX == 2) // zabojstwo
			{
				if (deltaMoveY == 2)
				{
					Piece p = board[(startX + endX) / 2, (startY + endY) / 2];
					if (p != null && p.isWhite != isWhite)
						return true;
				}
			}
		}

		if (!isWhite || isKing)
		{
			if (deltaMoveX == 1) // zwykly ruch
			{
				if (deltaMoveY == -1)
					return true;
			}
			else if (deltaMoveX == 2) // zabojstwo
			{
				if (deltaMoveY == -2)
				{
					Piece p = board[(startX + endX) / 2, (startY + endY) / 2];
					if (p != null && p.isWhite != isWhite)
						return true;
				}
			}
		}

		return false;
	}
}
