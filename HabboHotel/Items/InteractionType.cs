namespace Akiled.HabboHotel.Items
{
    public enum InteractionType
    {
        NONE,
        GATE,
        POSTIT,
        PHOTO,
        roomeffect,
        MOODLIGHT,
        TROPHY,
        bed,
        scoreboard,
        vendingmachine,
        alert,
        onewaygate,
        loveshuffler,
        habbowheel,
        dice,
        bottle,
        TELEPORT,
        ARROW,
        rentals,
        pet,
        pool,
        roller,
        fbgate,
        iceskates,
        normslaskates,
        lowpool,
        haloweenpool,
        WALLPAPER,
        FLOOR,
        LANDSCAPE,
        football,
        footballgoalgreen,
        footballgoalyellow,
        footballgoalblue,
        footballgoalred,
        footballcountergreen,
        footballcounteryellow,
        footballcounterblue,
        footballcounterred,
        banzaigateblue,
        banzaigatered,
        banzaigateyellow,
        banzaigategreen,
        banzaifloor,
        banzaiscoreblue,
        banzaiscorered,
        banzaiscoreyellow,
        banzaiscoregreen,
        banzaitele,
        banzaipuck,
        banzaipyramid,
        banzaiblo,
        banzaiblob,
        ChronoTimer,
        freezeexit,
        freezeredcounter,
        freezebluecounter,
        freezeyellowcounter,
        freezegreencounter,
        freezeyellowgate,
        freezeredgate,
        freezegreengate,
        freezebluegate,
        freezetileblock,
        freezetile,
        jukebox,
        puzzlebox,
        triggertimer,
        triggerroomenter,
        triggergameend,
        triggergamestart,
        triggerrepeater,
        triggerrepeaterlong,
        triggeronusersay,
        triggercommand,
        triggercollisionuser,
        triggerscoreachieved,
        triggerstatechanged,
        triggerwalkonfurni,
        triggerwalkofffurni,
        triggercollision,
        actiongivescore,
        actionposreset,
        actionmoverotate,
        actionresettimer,
        actionshowmessage,
        actionteleportto,
        wf_act_endgame_team,
        wf_act_call_stacks,
        actiontogglestate,
        actionkickuser,
        actionflee,
        actionchase,
        collisioncase,
        collisionteam,
        actiongivereward,
        actionmovetodir,
        conditionfurnishaveusers,
        wf_cnd_stuff_is,
        wf_cnd_not_stuff_is,
        conditionstatepos,
        conditionstateposNegative,
        conditiontimelessthan,
        conditiontimemorethan,
        conditiontriggeronfurni,
        conditiontriggeronfurniNegative,
        conditionhasfurnionfurni,
        conditionhasfurnionfurniNegative,
        conditionfurnishavenousers,
        conditionactoringroup,
        conditionnotingroup,
        ringplate,
        colortile,
        colorwheel,
        floorswitch1,
        firegate,
        glassfoor,
        specialrandom,
        specialunseen,
        wire,
        wireCenter,
        wireCorner,
        wireSplitter,
        wireStandard,
        GIFT,
        MANNEQUIN,
        TONER,
        bot,
        PURCHASABLE_CLOTHING,
        adsbackground,
        BADGE_DISPLAY,
        BADGE_TROC,
        tvyoutube,
        pilemagic,
        superwired,
        superwiredcondition,
        jmphorse,
        GUILD_ITEM,
        GUILD_GATE,
        GUILD_FORUM,
        pressurepad,
        gnomebox,
        monstergraine,
        highscore,
        highscorepoints,
        vendingenablemachine,
        wf_trg_bot_reached_stf,
        wf_trg_bot_reached_avtr,
        wf_act_bot_clothes,
        wf_act_bot_teleport,
        wf_act_bot_follow_avatar,
        wf_act_bot_give_handitem,
        wf_act_bot_move,
        wf_act_bot_talk_to_avatar,
        wf_act_bot_talk,
        wf_cnd_has_handitem,
        wf_act_join_team,
        wf_act_leave_team,
        wf_act_give_score_tm,
        wf_cnd_actor_in_team,
        wf_cnd_not_in_team,
        wf_cnd_not_user_count,
        wf_cnd_user_count_in,
        CRACKABLE,
        LOVELOCK,
        EXTRABOX,
        DELUXEBOX,
        LEGENDBOX,
        BADGEBOX,
        EXCHANGE,

        HORSE_SADDLE_1,
        HORSE_SADDLE_2,
        HORSE_HAIRSTYLE,
        HORSE_BODY_DYE,
        HORSE_HAIR_DYE,
        CANNON,
        BADGE,

        TRAMPOLINE,
        TREADMILL,
        CROSSTRAINER,
        wf_act_user_move,
        PREFIX_NAME,
        PREFIX_COLOR,
        PREFIX_COLORNAME,
        PREFIX_SIZENAME,
        PREFIX_SIZETAG,
        tronco,
        mineria,
        CRAFTING,
        ROOMCAMERA,
    }

    public class InteractionTypes
    {
        public static InteractionType GetTypeFromString(string pType)
        {
            switch (pType)
            {
                case "NAME_SIZE":
                case "prefix_sizename":
                    return InteractionType.PREFIX_SIZENAME;
                case "TAG_SIZE":
                case "prefix_sizetag":
                    return InteractionType.PREFIX_SIZETAG;
                case "actiongivescore":
                case "wf_act_givepoints":
                    return InteractionType.actiongivescore;
                case "actionmoverotate":
                case "wf_act_moverotate":
                    return InteractionType.actionmoverotate;
                case "actionposreset":
                case "wf_act_matchfurni":
                    return InteractionType.actionposreset;
                case "actionresettimer":
                    return InteractionType.actionresettimer;
                case "actionshowmessage":
                case "wf_act_saymsg":
                    return InteractionType.actionshowmessage;
                case "actionteleportto":
                case "wf_act_moveuser":
                    return InteractionType.actionteleportto;
                case "actiontogglestate":
                case "wf_act_togglefurni":
                    return InteractionType.actiontogglestate;
                case "adsbackground":
                    return InteractionType.adsbackground;
                case "alert":
                    return InteractionType.alert;
                case "arrowplate":
                case "pressure_pad":
                case "pressurepad":
                    return InteractionType.pressurepad;
                case "badge":
                    return InteractionType.BADGE;
                case "badge_display":
                    return InteractionType.BADGE_DISPLAY;
                case "badge_troc":
                    return InteractionType.BADGE_TROC;
                case "badgebox":
                    return InteractionType.BADGEBOX;
                case "ball":
                case "football":
                    return InteractionType.football;
                case "banzaicounter":
                case "counter":
                case "freezetimer":
                    return InteractionType.ChronoTimer;
                case "banzaifloor":
                case "bb_patch":
                    return InteractionType.banzaifloor;
                case "banzaigateblue":
                case "bb_blue_gate":
                    return InteractionType.banzaigateblue;
                case "banzaigategreen":
                case "bb_green_gate":
                    return InteractionType.banzaigategreen;
                case "banzaigatered":
                case "bb_red_gate":
                    return InteractionType.banzaigatered;
                case "banzaigateyellow":
                case "bb_yellow_gate":
                    return InteractionType.banzaigateyellow;
                case "banzaipuck":
                    return InteractionType.banzaipuck;
                case "banzaipyramid":
                    return InteractionType.banzaipyramid;
                case "banzaiscoreblue":
                    return InteractionType.banzaiscoreblue;
                case "banzaiscoregreen":
                    return InteractionType.banzaiscoregreen;
                case "banzaiscorered":
                    return InteractionType.banzaiscorered;
                case "banzaiscoreyellow":
                    return InteractionType.banzaiscoreyellow;
                case "banzaitele":
                case "bb_teleport":
                    return InteractionType.banzaitele;
                case "bed":
                    return InteractionType.bed;
                case "bgupdater":
                    return InteractionType.TONER;
                case "blue_goal":
                case "footballgoalblue":
                    return InteractionType.footballgoalblue;
                case "bot":
                    return InteractionType.bot;
                case "bottle":
                    return InteractionType.bottle;
                case "cannon":
                    return InteractionType.CANNON;
                case "colortile":
                    return InteractionType.colortile;
                case "colorwheel":
                    return InteractionType.colorwheel;
                case "conditionfurnishaveusers":
                case "wf_cnd_furnis_hv_avtrs":
                    return InteractionType.conditionfurnishaveusers;
                case "conditionstatepos":
                    return InteractionType.conditionstatepos;
                case "conditiontimelessthan":
                    return InteractionType.conditiontimelessthan;
                case "conditiontimemorethan":
                    return InteractionType.conditiontimemorethan;
                case "conditiontriggeronfurni":
                case "wf_cnd_trggrer_on_frn":
                    return InteractionType.conditiontriggeronfurni;
                case "crackable":
                    return InteractionType.CRACKABLE;
                case "crafting":
                    return InteractionType.CRAFTING;
                case "crosstrainer":
                    return InteractionType.CROSSTRAINER;
                case "default":
                    return InteractionType.NONE;
                case "deluxebox":
                    return InteractionType.DELUXEBOX;
                case "dice":
                    return InteractionType.dice;
                case "dimmer":
                    return InteractionType.MOODLIGHT;
                case "exchange":
                    return InteractionType.EXCHANGE;
                case "extrabox":
                    return InteractionType.EXTRABOX;
                case "fbgate":
                    return InteractionType.fbgate;
                case "firegate":
                    return InteractionType.firegate;
                case "floor":
                    return InteractionType.FLOOR;
                case "floorswitch1":
                case "floorswitch2":
                    return InteractionType.floorswitch1;
                case "footballcounterblue":
                    return InteractionType.footballcounterblue;
                case "footballcountered":
                    return InteractionType.footballcounterred;
                case "footballcountergreen":
                    return InteractionType.footballcountergreen;
                case "footballcounteryellow":
                    return InteractionType.footballcounteryellow;
                case "footballgoalgreen":
                case "green_goal":
                    return InteractionType.footballgoalgreen;
                case "footballgoalred":
                case "red_goal":
                    return InteractionType.footballgoalred;
                case "footballgoalyellow":
                case "yellow_goal":
                    return InteractionType.footballgoalyellow;
                case "freezebluecounter":
                    return InteractionType.freezebluecounter;
                case "freezebluegate":
                    return InteractionType.freezebluegate;
                case "freezeexit":
                    return InteractionType.freezeexit;
                case "freezegreencounter":
                    return InteractionType.freezegreencounter;
                case "freezegreengate":
                    return InteractionType.freezegreengate;
                case "freezeredcounter":
                    return InteractionType.freezeredcounter;
                case "freezeredgate":
                    return InteractionType.freezeredgate;
                case "freezetile":
                    return InteractionType.freezetile;
                case "freezetileblock":
                    return InteractionType.freezetileblock;
                case "freezeyellowcounter":
                    return InteractionType.freezeyellowcounter;
                case "freezeyellowgate":
                    return InteractionType.freezeyellowgate;
                case "gate":
                    return InteractionType.GATE;
                case "gift":
                    return InteractionType.GIFT;
                case "glassfoor":
                    return InteractionType.glassfoor;
                case "gnomebox":
                    return InteractionType.gnomebox;
                case "groupfurni":
                    return InteractionType.GUILD_ITEM;
                case "groupgate":
                    return InteractionType.GUILD_GATE;
                case "guild_forum":
                    return InteractionType.GUILD_FORUM;
                case "habbowheel":
                    return InteractionType.habbowheel;
                case "haloweenpool":
                    return InteractionType.haloweenpool;
                case "highscore":
                    return InteractionType.highscore;
                case "hightscorepoints":
                    return InteractionType.highscorepoints;
                case "horse_body_dye":
                    return InteractionType.HORSE_BODY_DYE;
                case "horse_hair_dye":
                    return InteractionType.HORSE_HAIR_DYE;
                case "horse_hairstyle":
                    return InteractionType.HORSE_HAIRSTYLE;
                case "horse_saddle_1":
                    return InteractionType.HORSE_SADDLE_1;
                case "horse_saddle_2":
                    return InteractionType.HORSE_SADDLE_2;
                case "iceskates":
                    return InteractionType.iceskates;
                case "jmphorse":
                    return InteractionType.jmphorse;
                case "jukebox":
                    return InteractionType.jukebox;
                case "landscape":
                    return InteractionType.LANDSCAPE;
                case "legendbox":
                    return InteractionType.LEGENDBOX;
                case "lovelock":
                    return InteractionType.LOVELOCK;
                case "loveshuffler":
                    return InteractionType.loveshuffler;
                case "lowpool":
                    return InteractionType.lowpool;
                case "maniqui":
                    return InteractionType.MANNEQUIN;
                case "mineria":
                    return InteractionType.mineria;
                case "monstergraine":
                    return InteractionType.monstergraine;
                case "normalskates":
                    return InteractionType.normslaskates;
                case "onewaygate":
                    return InteractionType.onewaygate;
                case "pet":
                    return InteractionType.pet;
                case "photo":
                    return InteractionType.PHOTO;
                case "pilemagic":
                    return InteractionType.pilemagic;
                case "pool":
                case "water":
                    return InteractionType.pool;
                case "postit":
                    return InteractionType.POSTIT;
                case "prefix":
                case "prefix_name":
                    return InteractionType.PREFIX_NAME;
                case "prefix_color":
                    return InteractionType.PREFIX_COLOR;
                case "prefix_colorname":
                    return InteractionType.PREFIX_COLORNAME;
                case "puzzlebox":
                    return InteractionType.puzzlebox;
                case "rentals":
                    return InteractionType.rentals;
                case "ringplate":
                    return InteractionType.ringplate;
                case "roller":
                    return InteractionType.roller;
                case "roomeffect":
                    return InteractionType.roomeffect;
                case "scoreboard":
                    return InteractionType.scoreboard;
                case "specialrandom":
                case "wf_xtra_random":
                    return InteractionType.specialrandom;
                case "specialunseen":
                case "wf_xtra_unseen":
                    return InteractionType.specialunseen;
                case "superwired":
                    return InteractionType.superwired;
                case "superwiredcondition":
                    return InteractionType.superwiredcondition;
                case "teleport":
                    return InteractionType.TELEPORT;
                case "teleportfloor":
                    return InteractionType.ARROW;
                case "trampoline":
                    return InteractionType.TRAMPOLINE;
                case "treadmill":
                    return InteractionType.TREADMILL;
                case "triggergameend":
                case "wf_trg_gameend":
                    return InteractionType.triggergameend;
                case "triggergamestart":
                case "wf_trg_gamestart":
                    return InteractionType.triggergamestart;
                case "triggeronusersay":
                case "wf_trg_onsay":
                    return InteractionType.triggeronusersay;
                case "triggerrepeater":
                case "wf_trg_timer":
                    return InteractionType.triggerrepeater;
                case "triggerroomenter":
                case "wf_trg_enterroom":
                    return InteractionType.triggerroomenter;
                case "triggerscoreachieved":
                case "wf_trg_atscore":
                    return InteractionType.triggerscoreachieved;
                case "triggerstatechanged":
                case "wf_trg_furnistate":
                    return InteractionType.triggerstatechanged;
                case "triggertimer":
                case "wf_trg_attime":
                    return InteractionType.triggertimer;
                case "triggerwalkofffurni":
                case "wf_trg_offfurni":
                    return InteractionType.triggerwalkofffurni;
                case "triggerwalkonfurni":
                case "wf_trg_onfurni":
                    return InteractionType.triggerwalkonfurni;
                case "tronco":
                    return InteractionType.tronco;
                case "trophy":
                    return InteractionType.TROPHY;
                case "tvyoutube":
                    return InteractionType.tvyoutube;
                case "vendingenablemachine":
                    return InteractionType.vendingenablemachine;
                case "vendingmachine":
                    return InteractionType.vendingmachine;
                case "wallpaper":
                    return InteractionType.WALLPAPER;
                case "wf_act_bot_clothes":
                    return InteractionType.wf_act_bot_clothes;
                case "wf_act_bot_follow_avatar":
                    return InteractionType.wf_act_bot_follow_avatar;
                case "wf_act_bot_give_handitem":
                    return InteractionType.wf_act_bot_give_handitem;
                case "wf_act_bot_move":
                    return InteractionType.wf_act_bot_move;
                case "wf_act_bot_talk":
                    return InteractionType.wf_act_bot_talk;
                case "wf_act_bot_talk_to_avatar":
                    return InteractionType.wf_act_bot_talk_to_avatar;
                case "wf_act_bot_teleport":
                    return InteractionType.wf_act_bot_teleport;
                case "wf_act_call_stacks":
                    return InteractionType.wf_act_call_stacks;
                case "wf_act_chase":
                    return InteractionType.actionchase;
                case "wf_act_collisioncase":
                    return InteractionType.collisioncase;
                case "wf_act_collisionteam":
                    return InteractionType.collisionteam;
                case "wf_act_endgame_team":
                    return InteractionType.wf_act_endgame_team;
                case "wf_act_flee":
                    return InteractionType.actionflee;
                case "wf_act_give_reward":
                    return InteractionType.actiongivereward;
                case "wf_act_give_score_tm":
                    return InteractionType.wf_act_give_score_tm;
                case "wf_act_join_team":
                    return InteractionType.wf_act_join_team;
                case "wf_act_kick":
                case "wf_act_kick_user":
                case "wf_act_kickuser":
                    return InteractionType.actionkickuser;
                case "wf_act_leave_team":
                    return InteractionType.wf_act_leave_team;
                case "wf_act_move_to_dir":
                    return InteractionType.actionmovetodir;
                case "wf_act_user_move":
                    return InteractionType.wf_act_user_move;
                case "wf_blob":
                    return InteractionType.banzaiblob;
                case "wf_blob2":
                    return InteractionType.banzaiblo;
                case "wf_cnd_actor_in_group":
                    return InteractionType.conditionactoringroup;
                case "wf_cnd_actor_in_team":
                    return InteractionType.wf_cnd_actor_in_team;
                case "wf_cnd_furnis_hv_prsn":
                case "wf_cnd_not_hv_avtrs":
                    return InteractionType.conditionfurnishavenousers;
                case "wf_cnd_has_furni_on":
                    return InteractionType.conditionhasfurnionfurni;
                case "wf_cnd_has_handitem":
                    return InteractionType.wf_cnd_has_handitem;
                case "wf_cnd_not_furni_on":
                    return InteractionType.conditionhasfurnionfurniNegative;
                case "wf_cnd_not_in_group":
                    return InteractionType.conditionnotingroup;
                case "wf_cnd_not_in_team":
                    return InteractionType.wf_cnd_not_in_team;
                case "wf_cnd_not_match_snap":
                    return InteractionType.conditionstateposNegative;
                case "wf_cnd_not_stuff_is":
                    return InteractionType.wf_cnd_not_stuff_is;
                case "wf_cnd_not_trggrer_on":
                    return InteractionType.conditiontriggeronfurniNegative;
                case "wf_cnd_not_user_count":
                    return InteractionType.wf_cnd_not_user_count;
                case "wf_cnd_stuff_is":
                    return InteractionType.wf_cnd_stuff_is;
                case "wf_cnd_user_count_in":
                    return InteractionType.wf_cnd_user_count_in;
                case "wf_trg_bot_reached_avtr":
                    return InteractionType.wf_trg_bot_reached_avtr;
                case "wf_trg_bot_reached_stf":
                    return InteractionType.wf_trg_bot_reached_stf;
                case "wf_trg_cls_user":
                    return InteractionType.triggercollisionuser;
                case "wf_trg_cmd":
                    return InteractionType.triggercommand;
                case "wf_trg_collision":
                    return InteractionType.triggercollision;
                case "wf_trg_period_long":
                    return InteractionType.triggerrepeaterlong;
                case "wire":
                    return InteractionType.wire;
                case "purchasable_clothing":
                case "PURCHASABLE_CLOTHING":
                case "clothing":
                    return InteractionType.PURCHASABLE_CLOTHING;
                case "wireCenter":
                    return InteractionType.wireCenter;
                case "wireCorner":
                    return InteractionType.wireCorner;
                case "wireSplitter":
                    return InteractionType.wireSplitter;
                case "wireStandard":
                    return InteractionType.wireStandard;
                default:
                    return InteractionType.NONE;
            }
        }
    }
}
