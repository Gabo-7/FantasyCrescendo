﻿// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

public class Yukkuri : MonoBehaviour {
    public string Reimu {
        get {
            return "  　＿_　　 _____　　 ＿_____" + "　,´　_,, '-´￣￣｀-ゝ 、_ イ、"
                + "　'r ´　　　　　　　　　　ヽ、ﾝ、" + "　,'＝=─-　　　 　 -─=＝',　i"
                + "i　ｲ　iゝ、ｲ人レ／_ルヽｲ i　|" + "ﾚﾘｲi (ﾋ_] 　　 　ﾋ_ﾝ ).| .|、i .||"
                + "　!Y!\"\"　 ,＿__, 　 \"\" 「 !ﾉ i　|"
                + "　 L.',.　 　ヽ _ﾝ　　　　L」 ﾉ| .|" + "   | ||ヽ、　　　　　　 ,ｲ| ||ｲ| /"
                + " 　レ ル｀ ー--─ ´ルﾚ　ﾚ´";
        }
    }

    public string Marisa {
        get {
            return "　　 _,,....,,_" + "-''\":::::::::::::｀'',__"
                + "ヽ::::::::::::::::::::::::::＼,_"
                + "　|::::::;ノ´￣＼:::::::::::＼_,. -‐ｧ"
                + "　|::::ﾉ　　　ヽ､ヽr-r'\"´　　（.__" + "_,.!イ_　　_,.ﾍｰｧ'二ﾊ二ヽ､へ,_7"
                + "::::::rｰ''7ｺ-‐'\"´　 　 ;　 ',　｀ヽ/｀7"
                + "r-'ｧ'\"´/　 /!　ﾊ 　ハ　 !　　iヾ_ﾉ"
                + "!イ´ ,' |　/__,.!/　V　､!__ﾊ　 ,'　,ゝ"
                + "`! 　!/ﾚi'　(ﾋ_] 　　 　ﾋ_ﾝ ﾚ'i　ﾉ"
                + ",'　 ﾉ 　 !'\"　 　 ,＿__,　 \"' i .ﾚ'" + "　（　　,ﾊ　　　　ヽ _ﾝ　 　人!"
                + ",.ﾍ,）､　　）＞,､ _____,　,.イ　 ハ";
        }
    }

    public string TakeItEasy() {
        return "　　 _,,....,,_　 ＿人人人人人人人人人人人人人人人＿"
            + "-''\":::::::::::::｀''＞　　　ゆっくりしていってね！！！　　　＜"
            + "ヽ:::::::::::::::::::::￣^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^Ｙ^￣"
            + "　|::::::;ノ´￣＼:::::::::::＼_,. -‐ｧ　　　　　＿_　　 _____　　 ＿_____"
            + "　|::::ﾉ　　　ヽ､ヽr-r'\"´　　（.__　　　　,´　_,, '-´￣￣｀-ゝ 、_ イ、"
            + "_,.!イ_　　_,.ﾍｰｧ'二ﾊ二ヽ､へ,_7　　　'r ´　　　　　　　　　　ヽ、ﾝ、"
            + "::::::rｰ''7ｺ-‐'\"´　 　 ;　 ',　｀ヽ/｀7　,'＝=─-　　　 　 -─=＝',　i"
            + "r-'ｧ'\"´/　 /!　ﾊ 　ハ　 !　　iヾ_ﾉ　i　ｲ　iゝ、ｲ人レ／_ルヽｲ i　|"
            + "!イ´ ,' |　/__,.!/　V　､!__ﾊ　 ,'　,ゝ　ﾚﾘｲi (ﾋ_] 　　 　ﾋ_ﾝ ).| .|、i .||"
            + "`! 　!/ﾚi'　(ﾋ_] 　　 　ﾋ_ﾝ ﾚ'i　ﾉ　　　!Y!\"\"　 ,＿__, 　 \"\" 「 !ﾉ i　|"
            + ",'　 ﾉ 　 !'\"　 　 ,＿__,　 \"' i .ﾚ'　　　　L.',.　 　ヽ _ﾝ　　　　L」 ﾉ| .|"
            + "　（　　,ﾊ　　　　ヽ _ﾝ　 　人! 　　　　 | ||ヽ、　　　　　　 ,ｲ| ||ｲ| /"
            + ",.ﾍ,）､　　）＞,､ _____,　,.イ　 ハ　　　　レ ル｀ ー--─ ´ルﾚ　ﾚ´";
    }

    public override string ToString() { return "ゆっくりしていってね！！！"; }
}