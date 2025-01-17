using System;

public class BachTuoc : Mob, IMapObject
{
	public static Image shadowBig = GameCanvas.loadImage("/mainImage/shadowBig.png");

	public static EffectData data;

	public int xTo;

	public int yTo;

	public bool haftBody;

	public bool change;

	private Mob mob1;

	public new int xSd;

	public new int ySd;

	private bool isOutMap;

	private int wCount;

	public new bool isShadown = true;

	private int tick;

	private int frame;

	public new static Image imgHP = GameCanvas.loadImage("/mainImage/myTexture2dmobHP.png");

	private bool wy;

	private int wt;

	private int fy;

	private int ty;

	public new int typeSuperEff;

	private Char focus;

	private bool flyUp;

	private bool flyDown;

	private int dy;

	public bool changePos;

	private int tShock;

	public new bool isBusyAttackSomeOne = true;

	private int tA;

	private Char[] charAttack;

	private int[] dameHP;

	private sbyte type;

	public new int[] stand = new int[12]
	{
		0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
		1, 1
	};

	public int[] movee = new int[12]
	{
		0, 0, 0, 2, 2, 2, 3, 3, 3, 4,
		4, 4
	};

	public new int[] attack1 = new int[12]
	{
		0, 0, 0, 4, 4, 4, 5, 5, 5, 6,
		6, 6
	};

	public new int[] attack2 = new int[17]
	{
		0, 0, 0, 7, 7, 7, 8, 8, 8, 9,
		9, 9, 10, 10, 10, 11, 11
	};

	public new int[] hurt = new int[4] { 1, 1, 7, 7 };

	private bool shock;

	private sbyte[] cou = new sbyte[2] { -1, 1 };

	public new Char injureBy;

	public new bool injureThenDie;

	public new Mob mobToAttack;

	public new int forceWait;

	public new bool blindEff;

	public new bool sleepEff;

	public BachTuoc(int id, short px, short py, int templateID, int hp, int maxHp, int s)
	{
		mobId = id;
		xFirst = (x = px + 20);
		yFirst = (y = py);
		xTo = x;
		yTo = y;
		base.maxHp = maxHp;
		base.hp = hp;
		templateId = templateID;
		getDataB();
		status = 2;
	}

	public void getDataB()
	{
		data = null;
		data = new EffectData();
		string patch = "/x" + mGraphics.zoomLevel + "/effectdata/" + 108 + "/data";
		try
		{
			data.readData2(patch);
			data.img = GameCanvas.loadImage("/effectdata/" + 108 + "/img.png");
		}
		catch (Exception)
		{
			Service.gI().requestModTemplate(templateId);
		}
		w = data.width;
		h = data.height;
	}

	public new void setBody(short id)
	{
		changBody = true;
		smallBody = id;
	}

	public new void clearBody()
	{
		changBody = false;
	}

	public new static bool isExistNewMob(string id)
	{
		for (int i = 0; i < Mob.newMob.size(); i++)
		{
			string text = (string)Mob.newMob.elementAt(i);
			if (text.Equals(id))
			{
				return true;
			}
		}
		return false;
	}

	public new void checkFrameTick(int[] array)
	{
		tick++;
		if (tick > array.Length - 1)
		{
			tick = 0;
		}
		frame = array[tick];
	}

	private void updateShadown()
	{
		int num = TileMap.size;
		xSd = x;
		wCount = 0;
		if (ySd <= 0 || TileMap.tileTypeAt(xSd, ySd, 2))
		{
			return;
		}
		if (TileMap.tileTypeAt(xSd / num, ySd / num) == 0)
		{
			isOutMap = true;
		}
		else if (TileMap.tileTypeAt(xSd / num, ySd / num) != 0 && !TileMap.tileTypeAt(xSd, ySd, 2))
		{
			xSd = x;
			ySd = y;
			isOutMap = false;
		}
		while (isOutMap && wCount < 10)
		{
			wCount++;
			ySd += 24;
			if (TileMap.tileTypeAt(xSd, ySd, 2))
			{
				if (ySd % 24 != 0)
				{
					ySd -= ySd % 24;
				}
				break;
			}
		}
	}

	private void paintShadow(mGraphics g)
	{
		int num = TileMap.size;
		g.drawImage(shadowBig, xSd, yFirst, 3);
		g.setClip(GameScr.cmx, GameScr.cmy - GameCanvas.transY, GameScr.gW, GameScr.gH + 2 * GameCanvas.transY);
	}

	public new void updateSuperEff()
	{
	}

	public override void update()
	{
		if (!isUpdate())
		{
			return;
		}
		updateShadown();
		switch (status)
		{
		case 2:
			updateMobStandWait();
			break;
		case 3:
			updateMobAttack();
			break;
		case 5:
			timeStatus = 0;
			updateMobWalk();
			break;
		case 6:
			timeStatus = 0;
			p1++;
			y += p1;
			if (y >= yFirst)
			{
				y = yFirst;
				p1 = 0;
				status = 5;
			}
			break;
		case 7:
			updateInjure();
			break;
		case 0:
		case 1:
			updateDead();
			break;
		case 4:
			break;
		}
	}

	private void updateDead()
	{
		checkFrameTick(stand);
		if (GameCanvas.gameTick % 5 == 0)
		{
			ServerEffect.addServerEffect(167, Res.random(x - getW() / 2, x + getW() / 2), Res.random(getY() + getH() / 2, getY() + getH()), 1);
		}
		if (x != xTo || y != yTo)
		{
			x += (xTo - x) / 4;
			y += (yTo - y) / 4;
		}
	}

	public new void setInjure()
	{
	}

	public new void setAttack(Char cFocus)
	{
		isBusyAttackSomeOne = true;
		mobToAttack = null;
		base.cFocus = cFocus;
		p1 = 0;
		p2 = 0;
		status = 3;
		tick = 0;
		dir = ((cFocus.cx > x) ? 1 : (-1));
		int cx = cFocus.cx;
		int cy = cFocus.cy;
		if (Res.abs(cx - x) < w * 2 && Res.abs(cy - y) < h * 2)
		{
			if (x < cx)
			{
				x = cx - w;
			}
			else
			{
				x = cx + w;
			}
			p3 = 0;
		}
		else
		{
			p3 = 1;
		}
	}

	private bool isSpecial()
	{
		if ((templateId >= 58 && templateId <= 65) || templateId == 67 || templateId == 68)
		{
			return true;
		}
		return false;
	}

	private void updateInjure()
	{
	}

	private void updateMobStandWait()
	{
		checkFrameTick(stand);
		if (x != xTo || y != yTo)
		{
			x += (xTo - x) / 4;
			y += (yTo - y) / 4;
		}
	}

	public void setFly()
	{
		status = 4;
		flyUp = true;
	}

	public void setAttack(Char[] cAttack, int[] dame, sbyte type)
	{
		charAttack = cAttack;
		dameHP = dame;
		this.type = type;
		status = 3;
	}

	public new void updateMobAttack()
	{
		if (type == 3)
		{
			if (tick == attack1.Length - 1)
			{
				status = 2;
			}
			dir = ((x < charAttack[0].cx) ? 1 : (-1));
			checkFrameTick(attack1);
			x += (charAttack[0].cx - x) / 4;
			y += (charAttack[0].cy - y) / 4;
			xTo = x;
			if (tick == 8)
			{
				for (int i = 0; i < charAttack.Length; i++)
				{
					charAttack[i].doInjure(dameHP[i], 0, false, false);
					ServerEffect.addServerEffect(102, charAttack[i].cx, charAttack[i].cy, 1);
				}
			}
		}
		if (type != 4)
		{
			return;
		}
		if (tick == attack2.Length - 1)
		{
			status = 2;
		}
		dir = ((x < charAttack[0].cx) ? 1 : (-1));
		checkFrameTick(attack2);
		if (tick == 8)
		{
			for (int j = 0; j < charAttack.Length; j++)
			{
				charAttack[j].doInjure(dameHP[j], 0, false, false);
				ServerEffect.addServerEffect(102, charAttack[j].cx, charAttack[j].cy, 1);
			}
		}
	}

	public new void updateMobWalk()
	{
		checkFrameTick(movee);
		x += ((x >= xTo) ? (-2) : 2);
		y = yTo;
		dir = ((x < xTo) ? 1 : (-1));
		if (Res.abs(x - xTo) <= 1)
		{
			x = xTo;
			status = 2;
		}
	}

	public new bool isPaint()
	{
		if (x < GameScr.cmx)
		{
			return false;
		}
		if (x > GameScr.cmx + GameScr.gW)
		{
			return false;
		}
		if (y < GameScr.cmy)
		{
			return false;
		}
		if (y > GameScr.cmy + GameScr.gH + 30)
		{
			return false;
		}
		if (status == 0)
		{
			return false;
		}
		return true;
	}

	public new bool isUpdate()
	{
		if (status == 0)
		{
			return false;
		}
		return true;
	}

	public new bool checkIsBoss()
	{
		if (isBoss || levelBoss > 0)
		{
			return true;
		}
		return false;
	}

	public override void paint(mGraphics g)
	{
		if (data == null)
		{
			return;
		}
		if (isShadown && status != 0)
		{
			paintShadow(g);
		}
		g.translate(0, GameCanvas.transY);
		data.paintFrame(g, frame, x, y + fy, (dir != 1) ? 1 : 0, 2);
		g.translate(0, -GameCanvas.transY);
		int num = (int)((long)hp * 50L / maxHp);
		if (num != 0)
		{
			g.setColor(0);
			g.fillRect(x - 27, y - 82, 54, 8);
			g.setColor(16711680);
			g.setClip(x - 25, y - 80, num, 4);
			g.fillRect(x - 25, y - 80, 50, 4);
			g.setClip(0, 0, 3000, 3000);
		}
		if (shock)
		{
			tShock++;
			Effect me = new Effect((type != 2) ? 22 : 19, x + tShock * 50, y + 25, 2, 1, -1);
			EffecMn.addEff(me);
			Effect me2 = new Effect((type != 2) ? 22 : 19, x - tShock * 50, y + 25, 2, 1, -1);
			EffecMn.addEff(me2);
			if (tShock == 50)
			{
				tShock = 0;
				shock = false;
			}
		}
	}

	public new int getHPColor()
	{
		return 16711680;
	}

	public new void startDie()
	{
		hp = 0;
		injureThenDie = true;
		hp = 0;
		status = 1;
		p1 = -3;
		p2 = -dir;
		p3 = 0;
	}

	public new void attackOtherMob(Mob mobToAttack)
	{
		this.mobToAttack = mobToAttack;
		isBusyAttackSomeOne = true;
		cFocus = null;
		p1 = 0;
		p2 = 0;
		status = 3;
		tick = 0;
		dir = ((mobToAttack.x > x) ? 1 : (-1));
		int num = mobToAttack.x;
		int num2 = mobToAttack.y;
		if (Res.abs(num - x) < w * 2 && Res.abs(num2 - y) < h * 2)
		{
			if (x < num)
			{
				x = num - w;
			}
			else
			{
				x = num + w;
			}
			p3 = 0;
		}
		else
		{
			p3 = 1;
		}
	}

	public new int getX()
	{
		return x;
	}

	public new int getY()
	{
		return y - 40;
	}

	public new int getH()
	{
		return 40;
	}

	public new int getW()
	{
		return 40;
	}

	public new void stopMoving()
	{
		if (status == 5)
		{
			status = 2;
			p1 = (p2 = (p3 = 0));
			forceWait = 50;
		}
	}

	public new bool isInvisible()
	{
		return status == 0 || status == 1;
	}

	public new void removeHoldEff()
	{
		if (holdEffID != 0)
		{
			holdEffID = 0;
		}
	}

	public new void removeBlindEff()
	{
		blindEff = false;
	}

	public new void removeSleepEff()
	{
		sleepEff = false;
	}

	public new void move(short xMoveTo)
	{
		xTo = xMoveTo;
		status = 5;
	}
}
