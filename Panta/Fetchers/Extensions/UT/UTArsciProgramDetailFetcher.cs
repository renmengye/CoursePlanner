﻿using Panta.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Panta.Fetchers.Extensions.UT
{
    public class UTArtsciProgramDetailFetcher : WebpageItemFetcher<SchoolProgram>
    {
        public UTArtsciProgramDetailFetcher(string url) : base(url) { }

        private static Regex DetailRegex, AngleRegex, CodeRegex, StartRegex, BrRegex;

        static UTArtsciProgramDetailFetcher()
        {
            StartRegex = new Regex("((<span class=\"heading2\">)|(<h2>)).[^<]*?Programs((</span>)|(</h2))(?<content>.*?)(?=<h2>)");
            DetailRegex = new Regex("<span class=\"strong\">(?<name>[^<]*)</span>(?<content>.*?)(?=(<span class=\"strong)|(<a name)|(<h2>)|($))");
            AngleRegex = new Regex("<[^>]+>", RegexOptions.Compiled);
            CodeRegex = new Regex("(?<code>[A-Z]{3}[0-9]{3})(?<prefix>[HY][0-9])", RegexOptions.Compiled);
            BrRegex = new Regex("<br[^>]*>", RegexOptions.Compiled);
        }

        public override IEnumerable<SchoolProgram> FetchItems()
        {
            List<SchoolProgram> results = new List<SchoolProgram>();
            TextInfo formatter = CultureInfo.CurrentCulture.TextInfo;
            if (this.Content != null)
            {
                this.Content = this.Content.Replace("\r", String.Empty);
                this.Content = this.Content.Replace("\n", String.Empty);
                Match contentMatch = StartRegex.Match(this.Content);
                if (contentMatch.Success)
                {
                    MatchCollection matches = DetailRegex.Matches(contentMatch.Groups["content"].ToString());
                    foreach (Match match in matches)
                    {
                        string name = match.Groups["name"].ToString();
                        string content = match.Groups["content"].ToString();

                        name = HttpUtility.HtmlDecode(name);

                        // Exceptions with course groups
                        if (name.StartsWith("Psychology"))
                        {
                            content += "<br\\><a name=\"Psychology Course Groups\"></a><br><span class=\"heading3\">Psychology Course Groups</span><p><strong>Group 1 (Courses offered through the Psychology Department):</strong><br /><br />  <strong><img src=\"../gifs/new.gif\" width=\"28\" height=\"11\" alt=\"NEW\"></strong>Cluster A (Courses with a focus on Social/Personality/Developmental/Abnormal Psychology):<br />  <a href=\"crs_lin.htm#JLP315H1\">JLP315H1</a>; <a href=\"crs_psy.htm#PSY210H1\">PSY210H1</a>/<a href=\"crs_psy.htm#PSY220H1\">PSY220H1</a>/<a href=\"crs_psy.htm#PSY230H1\">PSY230H1</a>/<a href=\"crs_psy.htm#PSY240H1\">PSY240H1</a>/<a href=\"crs_psy.htm#PSY299Y1\">PSY299Y1</a>/<a href=\"crs_psy.htm#PSY307H1\">PSY307H1</a>/<a href=\"crs_psy.htm#PSY308H1\">PSY308H1</a>/ <a href=\"crs_psy.htm#PSY311H1\">PSY311H1</a>/<a href=\"crs_psy.htm#PSY312H1\">PSY312H1</a>/<a href=\"crs_psy.htm#PSY313H1\">PSY313H1</a>/<a href=\"crs_psy.htm#PSY316H1\">PSY316H1</a>/<a href=\"crs_psy.htm#PSY319H1\">PSY319H1</a>/<a href=\"crs_psy.htm#PSY320H1\">PSY320H1</a>/<a href=\"crs_psy.htm#PSY321H1\">PSY321H1</a>/<a href=\"crs_psy.htm#PSY322H1\">PSY322H1</a>/<a href=\"crs_psy.htm#PSY323H1\">PSY323H1</a>/<a href=\"crs_psy.htm#PSY326H1\">PSY326H1</a>/<a href=\"crs_psy.htm#PSY328H1\">PSY328H1</a>/<a href=\"crs_psy.htm#PSY329H1\">PSY329H1</a>/<a href=\"crs_psy.htm#PSY330H1\">PSY330H1</a>/<a href=\"crs_psy.htm#PSY331H1\">PSY331H1</a>/<a href=\"crs_psy.htm#PSY332H1\">PSY332H1</a>/<a href=\"crs_psy.htm#PSY333H1\">PSY333H1</a>/<a href=\"crs_psy.htm#PSY336H1\">PSY336H1</a>/<a href=\"crs_psy.htm#PSY337H1\">PSY337H1</a>/<a href=\"crs_psy.htm#PSY339H1\">PSY339H1</a>/<a href=\"crs_psy.htm#PSY341H1\">PSY341H1</a>/<a href=\"crs_psy.htm#PSY342H1\">PSY342H1</a>/<a href=\"crs_psy.htm#PSY343H1\">PSY343H1</a>/<a href=\"crs_psy.htm#PSY402H1\">PSY402H1</a>/<a href=\"crs_psy.htm#PSY403H1\">PSY403H1</a>/<a href=\"crs_psy.htm#PSY404H1\">PSY404H1</a>/<a href=\"crs_psy.htm#PSY405H1\">PSY405H1</a>/<a href=\"crs_psy.htm#PSY406H1\">PSY406H1</a>/<a href=\"crs_psy.htm#PSY407H1\">PSY407H1</a>/<a href=\"crs_psy.htm#PSY408H1\">PSY408H1</a>/<a href=\"crs_psy.htm#PSY410H1\">  PSY410H1</a>/<a href=\"crs_psy.htm#PSY414H1\">PSY414H1</a> (formerly PSY314H1)/<a href=\"crs_psy.htm#PSY417H1\">PSY417H1</a> (formerly PSY317H1)/<a href=\"crs_psy.htm#PSY420H1\">PSY420H1</a>/PSY 421H1/<a href=\"crs_psy.htm#PSY424H1\">PSY424H1</a> (formerly PSY324H1)/<a href=\"crs_psy.htm#PSY425H1\">PSY425H1</a> (formerly PSY325H1)/<a href=\"crs_psy.htm#PSY426H1\">PSY426H1</a>/<a href=\"crs_psy.htm#PSY427H1\">PSY427H1</a> (formerly PSY327H1)/<a href=\"crs_psy.htm#PSY430H1\">PSY430H1</a>/<a href=\"crs_psy.htm#PSY434H1\">PSY434H1</a> (formerly PSY334H1)/<a href=\"crs_psy.htm#PSY435H1\">PSY435H1</a> (formerly PSY335H1)/<a href=\"crs_psy.htm#PSY440H1\">PSY440H1</a>/<a href=\"crs_psy.htm#PSY450H1\">PSY450H1</a> (formerly PSY300H1)</p><p><strong><img src=\"../gifs/new.gif\" width=\"28\" height=\"11\" alt=\"NEW\"></strong>Cluster B (Courses with a focus on Cognition/Perception/Learning/Brain and Behaviour):<br />  <a href=\"crs_lin.htm#JLP374H1\">JLP374H1</a>/<a href=\"crs_lin.htm#JLP471H1\">JLP471H1</a>,<a href=\"crs_psy.htm#PSY260H1\">PSY260H1</a>/<a href=\"crs_psy.htm#PSY270H1\">PSY270H1</a>/<a href=\"crs_psy.htm#PSY280H1\">PSY280H1</a>/<a href=\"crs_psy.htm#PSY290H1\">PSY290H1</a>/<a href=\"crs_psy.htm#PSY299Y1\">PSY299Y1</a>/<a href=\"crs_psy.htm#PSY307H1\">PSY307H1</a>/<a href=\"crs_psy.htm#PSY308H1\">PSY308H1</a>/<a href=\"crs_psy.htm#PSY312H1\">PSY312H1</a>/<a href=\"crs_psy.htm#PSY316H1\">PSY316H1</a>/<a href=\"crs_psy.htm#PSY362H1\">PSY362H1</a>/<a href=\"crs_psy.htm#PSY370H1\">PSY370H1</a>/<a href=\"crs_psy.htm#PSY371H1\">PSY371H1</a>/<a href=\"crs_psy.htm#PSY372H1\">PSY372H1</a>/<a href=\"crs_psy.htm#PSY378H1\">PSY378H1</a>/<a href=\"crs_psy.htm#PSY379H1\">PSY379H1</a>/<a href=\"crs_psy.htm#PSY380H1\">PSY380H1</a>/<a href=\"crs_psy.htm#PSY389H1\">PSY389H1</a>/<a href=\"crs_psy.htm#PSY390H1\">PSY390H1</a>/<a href=\"crs_psy.htm#PSY396H1\">PSY396H1</a>/<a href=\"crs_psy.htm#PSY397H1\">PSY397H1</a>/<a href=\"crs_psy.htm#PSY399H1\">PSY399H1</a>/<a href=\"crs_psy.htm#PSY402H1\">PSY402H1</a>/<a href=\"crs_psy.htm#PSY403H1\">PSY403H1</a>/<a href=\"crs_psy.htm#PSY404H1\">PSY404H1</a>/<a href=\"crs_psy.htm#PSY405H1\">PSY405H1</a>/<a href=\"crs_psy.htm#PSY406H1\">PSY406H1</a>/<a href=\"crs_psy.htm#PSY407H1\">PSY407H1</a>/<a href=\"crs_psy.htm#PSY408H1\">PSY408H1</a>/<a href=\"crs_psy.htm#PSY450H1\">PSY450H1</a> (formerly PSY300H1)&nbsp;/<a href=\"crs_psy.htm#PSY460H1\">PSY460H1</a>/<a href=\"crs_psy.htm#PSY470H1\">PSY470H1</a>/<a href=\"crs_psy.htm#PSY471H1\">PSY471H1</a>/<a href=\"crs_psy.htm#PSY473H1\">PSY473H1</a> (formerly PSY373H1)/<a href=\"crs_psy.htm#PSY475H1\">PSY475H1</a> (formerly PSY375H1)/<a href=\"crs_psy.htm#PSY480H1\">PSY480H1</a>/<a href=\"crs_psy.htm#PSY490H1\">PSY490H1</a>/<a href=\"crs_psy.htm#PSY492H1\">PSY492H1</a> (formerly PSY392H1)/<a href=\"crs_psy.htm#PSY493H1\">PSY493H1</a> (formerly PSY393H1)/<a href=\"crs_psy.htm#PSY494H1\">PSY494H1</a> (formerly PSY394H1)/<a href=\"crs_psy.htm#PSY497H1\">PSY497H1</a></p><p><strong><img src=\"../gifs/new.gif\" width=\"28\" height=\"11\" alt=\"NEW\"></strong>Some PSY courses are included in both of Clusters A  and B and may count in either cluster, but not both, for program requirements.  For Individual Projects or Special Topics courses being used to complete the  cluster requirement, please confirm group cluster with the Psychology  undergraduate office (<a href=\"crs_psy.htm#PSY405H1\">PSY405H1</a>/<a href=\"crs_psy.htm#PSY406H1\">PSY406H1</a> and  <a href=\"crs_psy.htm#PSY307H1\">PSY307H1</a>/<a href=\"crs_psy.htm#PSY308H1\">PSY308H1</a>/<a href=\"crs_psy.htm#PSY407H1\">PSY407H1</a>/<a href=\"crs_psy.htm#PSY408H1\">PSY408H1</a>). <br>  <a href=\"crs_psy.htm#PSY202H1\">PSY202H1</a> can be used towards program requirements as  well (under the final requirement for each program).<br />  <br />  <strong><img src=\"../gifs/new.gif\" width=\"28\" height=\"11\" alt=\"NEW\">Group 2 (Courses relevant to Psychology offered outside the Psychology Department):</strong><br /><br /><a href=\"crs_csb.htm#BIO130H1\">BIO130H1</a>/150Y1/252Y1/<a href=\"crs_csb.htm#BIO270H1\">BIO270H1</a>/<a href=\"crs_csb.htm#BIO271H1\">BIO271H1</a>; <a href=\"crs_cog.htm#COG250Y1\">COG250Y1</a> (formerly UNI250Y1); <a href=\"crs_csb.htm#CSB332H1\">CSB332H1</a>; <a href=\"crs_eng.htm#ENG384Y1\">ENG384Y1</a> (formerly ENG290Y1); <a href=\"crs_eth.htm#ETH220H1\">ETH220H1</a>; <a href=\"crs_his.htm#HIS489H1\">HIS489H1</a>; <a href=\"crs_hmb.htm#HMB200H1\">HMB200H1</a>/<a href=\"crs_hmb.htm#HMB220H1\">HMB220H1</a> (formerly HMB204H1)/<a href=\"crs_hmb.htm#HMB300H1\">HMB300H1</a>/<a href=\"crs_hmb.htm#HMB310H1\">HMB310H1</a>/<a href=\"crs_hmb.htm#HMB320H1\">HMB320H1</a>/<a href=\"crs_hmb.htm#HMB400Y1\">HMB400Y1</a>/<a href=\"crs_hmb.htm#HMB420H1\">HMB420H1</a>; <a href=\"crs_hps.htm#HPS200H1\">HPS200H1</a>; <a href=\"crs_abs.htm#JFP450H1\">JFP450H1</a>; <a href=\"crs_lin.htm#JLS474H1\">JLS474H1</a>; <a href=\"crs_lin.htm#LIN100Y1\">LIN100Y1</a>/<a href=\"crs_lin.htm#LIN200H1\">LIN200H1</a>; <a href=\"crs_new.htm#NEW232Y1\">NEW232Y1</a>/<a href=\"crs_new.htm#NEW302Y1\">NEW302Y1</a>/<a href=\"crs_new.htm#NEW303H1\">NEW303H1</a>/<a href=\"crs_new.htm#NEW332H1\">NEW332H1</a>/<a href=\"crs_new.htm#NEW333H1\">NEW333H1</a>/<a href=\"crs_new.htm#NEW336H1\">NEW336H1</a>/<a href=\"crs_new.htm#NEW339H1\">NEW339H1</a>/<a href=\"crs_new.htm#NEW433H1\">NEW433H1</a>/<a href=\"crs_new.htm#NEW438H1\">NEW438H1</a> (formerly NEW338H1); <a href=\"crs_pcl.htm#PCL475Y1\">PCL475Y1</a>; <a href=\"crs_phl.htm#PHL240H1\">PHL240H1</a>/<a href=\"crs_phl.htm#PHL243H1\">PHL243H1</a>/<a href=\"crs_phl.htm#PHL319H1\">PHL319H1</a>/<a href=\"crs_phl.htm#PHL340H1\">PHL340H1</a>/<a href=\"crs_phl.htm#PHL383H1\">PHL383H1</a>; <a href=\"crs_pol.htm#POL313Y1\">POL313Y1</a>; <a href=\"crs_psl.htm#PSL201Y1\">PSL201Y1</a>/<a href=\"crs_psl.htm#PSL300H1\">PSL300H1</a>/the former PSL302Y1/</span><a href=\"crs_psl.htm#PSL440Y1\">PSL440Y1</a>/<a href=\"crs_psl.htm#PSL444Y1\">PSL444Y1</a>; <a href=\"crs_rlg.htm#RLG211Y1\">RLG211Y1</a>/<a href=\"crs_rlg.htm#RLG301H1\">RLG301H1</a>/<a href=\"crs_rlg.htm#RLG302H1\">RLG302H1</a>/<a href=\"crs_rlg.htm#RLG421H1\">RLG421H1</a>; <a href=\"crs_rsm.htm#RSM260H1\">RSM260H1</a>/<a href=\"crs_rsm.htm#RSM353H1\">RSM353H1</a>; <a href=\"crs_soc.htm#SOC363H1\">SOC363H1</a>; <a href=\"crs_vic.htm#VIC261H1\">VIC261H1</a>; <a href=\"crs_wdw.htm#WDW260H1\">WDW260H1 </a>/ <a href=\"crs_wdw.htm#WDW360H1\">WDW360H1</a>/<a href=\"crs_wdw.htm#WDW365H1\">WDW365H1</a>/<a href=\"crs_wdw.htm#WDW431H1\">WDW431H1</a>; <a href=\"crs_wgs.htm#WGS372H1\">WGS372H1</a><br /><br />*Please note that the courses in Group 2 are optional and that enrolment priority is not given to PSY program students.</p>";
                        }
                        else if (name.StartsWith("Aboriginal"))
                        {
                            content += "<br\\><br\\><span class=\"heading3\">Aboriginal Studies Groups</span><p><strong>Group A:</strong></p><p><a href=\"crs_abs.htm#ABS210Y1\">ABS210Y1</a> Introduction to Anishinaabemowin<br /><a href=\"crs_abs.htm#ABS220Y1\">ABS220Y1</a> Introduction to an Iroquoian Language<br /><a href=\"crs_abs.htm#ABS230H1\">ABS230H1</a> Introduction to Inuktitut<br /><a href=\"crs_abs.htm#ABS231H1\">ABS231H1</a> Elementary Inuktitut<br /><a href=\"crs_abs.htm#ABS250H1\">ABS250H1</a> Indigenous Environmental Education<br /><a href=\"crs_abs.htm#ABS302H1\">ABS302H1</a> Aboriginal People in the Mass Media<br /><a href=\"crs_abs.htm#ABS310Y1\">ABS310Y1</a> Anishinaabemowin II<br /><a href=\"crs_abs.htm#ABS323Y1\">ABS323Y1</a> Intermediate Iroquoian Language<br /><a href=\"crs_abs.htm#ABS320Y1\">ABS320Y1</a>/<a href=\"crs_abs.htm#ABS321H1\">ABS321H1</a> Aboriginal Visual Expression: Technical and Theoretical Aspects<br /><a href=\"crs_abs.htm#ABS330Y1\">ABS330Y1</a>/<a href=\"crs_abs.htm#ABS331H1\">ABS331H1</a> Aboriginal Music: Technical and Theoretical Aspects<br /><a href=\"crs_abs.htm#ABS341H1\">ABS341H1</a> Indigenous Theatre<br /><a href=\"crs_abs.htm#ABS350Y1\">ABS350Y1</a> Aboriginal Health Systems<br /><a href=\"crs_abs.htm#ABS351Y1\">ABS351Y1</a> Aboriginal Legends and Teaching<br /><a href=\"crs_abs.htm#ABS353H1\">ABS353H1</a> First Nations&nbsp;Politics in Canada<br /><a href=\"crs_abs.htm#ABS354H1\">ABS354H1</a> Aboriginal Rights and Indigenous Law&nbsp;in Canada&nbsp;<br /><a href=\"crs_abs.htm#ABS360Y1\">ABS360Y1</a> Politics and Process of Reconciliation in Canada</p><p><a href=\"crs_abs.htm#ABS402H1\">ABS402H1</a> Traditional Indigenous Ecological Knowledge<br /><a href=\"crs_abs.htm#ABS405Y1\">ABS405Y1</a> Indigenous Thought and Expression: Creative Non-fiction<br /><a href=\"crs_abs.htm#ABS460Y1\">ABS460Y1</a> Methodology in Aboriginal Studies<br /><a href=\"crs_abs.htm#ABS495Y1\">ABS495Y1</a> Independent Research <br /><a href=\"crs_abs.htm#ABS496H1\">ABS496H1</a> Independent Research <br /><a href=\"crs_abs.htm#ABS497H1\">ABS497H1</a> Independent Research <br /><a href=\"crs_abs.htm#ABS498Y1\">ABS498Y1</a> Independent Research <br /><a href=\"crs_ant.htm#ANT315H1\">ANT315H1</a> Arctic Archaeology <br /><a href=\"crs_ant.htm#ANT365H1\">ANT365H1</a> Native America and the State<br /><a href=\"crs_ant.htm#ANT353H1\">ANT353H1</a> Anthropology of Indigeneity<br /><a href=\"crs_ant.htm#ANT463H1\">ANT463H1</a> Native Rights, Canadian Law <br /><a href=\"crs_eng.htm#ENG254Y1\">ENG254Y1</a> Indigenous Literatures of North America<br /><a href=\"crs_eng.htm#ENG355H1\">ENG355H1</a> Indigenous Women&rsquo;s Literature<br />HIS369Y1&nbsp;Aboriginal Peoples of the Great Lakes from 1500 to 1830&nbsp;<br /><a href=\"crs_his.htm#HIS472H1\">HIS472H1</a>&nbsp;Indigenous-Newcomer Relations in Canadian History</p><p>JPA461H1 Globalization and Indigenous Politics<br /><a href=\"crs_ggr.htm#GGR321H1\">GGR321H1</a> Aboriginal People and Environmental Issues in Canada (formerly known as JAG321H1)<br /><a href=\"crs_abs.htm#JFP450H1\">JFP450H1</a> Aboriginal Issues in Health and Healing (offered by the Faculty of Pharmacy)<br /><a href=\"crs_lin.htm#LIN458H1\">LIN458H1</a> Revitalizing Languages</p><p><strong>Group B:</strong></p><p><a href=\"crs_ant.htm#ANT200Y1\">ANT200Y1</a> Introduction to&nbsp;Archaeology <br /><a href=\"crs_ant.htm#ANT204H1\">ANT204H1</a>&nbsp;Anthropology of the Comtemporary World<br /><a href=\"crs_ant.htm#ANT319Y1\">ANT319Y1</a> Archaeology of North America<br /><a href=\"crs_ant.htm#ANT311Y1\">ANT311Y1</a> Archaeological Fieldwork<br /><a href=\"crs_ant.htm#ANT348H1\">ANT348H1</a> Anthropology of Health<br /><a href=\"crs_ant.htm#ANT367Y1\">ANT367Y1</a> Indigenous Spirituality<br /><a href=\"crs_ant.htm#ANT410H1\">ANT410H1</a> Hunter-Gatherers Past and Present <br /><a href=\"crs_ant.htm#ANT454H1\">ANT454H1</a> The Anthropology of Music &amp; Art<br /><a href=\"crs_for.htm#FOR200H1\">FOR200H1</a> Conservation of Canada&rsquo;s Forests<br /><a href=\"crs_his.htm#HIS106Y1\">HIS106Y1</a> Natives, Settlers, and Slaves: Colonizing the Americas, 1492-1804<br /><a href=\"crs_his.htm#HIS294Y1\">HIS294Y1</a> Caribbean History and Culture: Indigenous Era to 1886<br /><a href=\"crs_his.htm#HIS358H1\">HIS358H1</a> How the West was Colonized<br /><a href=\"crs_his.htm#HIS384H1\">HIS384H1</a> Colonial Canada<br /><a href=\"crs_ini.htm#INI327Y1\">INI327Y1</a> Screening Race (pre-requisite required)<br /><a href=\"crs_lin.htm#LIN351H1\">LIN351H1</a> Sociolinguistic Patterns in Language<br /><a href=\"crs_new.htm#NEW224Y1\">NEW224Y1</a> Caribbean Thought I<br /><a href=\"crs_new.htm#NEW240Y1\">NEW240Y1</a> Introduction to Equity Studies<br /><a href=\"crs_new.htm#NEW324Y1\">NEW324Y1</a> Caribbean Thought I</p><p>&nbsp;</p>";
                        }
                        else if (name.StartsWith("African"))
                        {
                            content += "<br\\><br\\><span class=\"heading3\">African Studies Course Groups</span><p><strong>Group A </strong>(Courses that deal exclusively with Africa. These include but are not limited to the following):<br /><a href=\"crs_his.htm#HIS297Y1\">HIS297Y1</a>, <a href=\"crs_his.htm#HIS383H1\">HIS383H1</a>, <a href=\"crs_his.htm#HIS481H1\">HIS481H1</a>; <a href=\"crs_hmb.htm#JNH350H1\">JNH350H1</a>; <a href=\"crs_dts.htm#JQR360H1\">JQR360H1</a>; <a href=\"crs_new.htm#NEW250Y1\">NEW250Y1</a>, <a href=\"crs_new.htm#NEW258H1\">NEW258H1</a>, <a href=\"crs_new.htm#NEW322H1\">NEW322H1</a>, <a href=\"crs_new.htm#NEW351Y1\">NEW351Y1</a>, <a href=\"crs_new.htm#NEW352H1\">NEW352H1</a>, <a href=\"crs_new.htm#NEW359H1\">NEW359H1</a>, <a href=\"crs_new.htm#NEW450Y1\">NEW450Y1</a>, <a href=\"crs_new.htm#NEW451H1\">NEW451H1</a>, <a href=\"crs_new.htm#NEW452H1\">NEW452H1</a>, <a href=\"crs_new.htm#NEW453Y1\">NEW453Y1</a>; <a href=\"crs_pol.htm#POL301Y1\">POL301Y1</a>, <a href=\"crs_pol.htm#POL488Y1\">POL488Y1</a>, <a href=\"crs_pol.htm#POL489H1\">POL489H1</a>; <a href=\"crs_smc.htm#SMC209H1\">SMC209H1</a>; an independent studies course approved by the Program Committee<br /><br /><strong>Group B </strong>(Courses that deal with Africa and/or one or more of its diaspora. These include but are not limited to the following):<br /><a href=\"crs_ant.htm#ANT204H1\">ANT204H1</a>, <a href=\"crs_ant.htm#ANT345H1\">ANT345H1</a>, <a href=\"crs_ant.htm#ANT363Y1\">ANT363Y1</a>, <a href=\"crs_ant.htm#ANT367Y1\">ANT367Y1</a>, <a href=\"crs_ant.htm#ANT426H1\">ANT426H1</a>, <a href=\"crs_ant.htm#ANT454H1\">ANT454H1</a>; <a href=\"crs_arc.htm#ARC233H1\">ARC233H1</a>; <a href=\"crs_dts.htm#DTS200Y1\">DTS200Y1</a>, <a href=\"crs_dts.htm#DTS401H1\">DTS401H1</a>, <a href=\"crs_dts.htm#DTS402H1\">DTS402H1</a>; <a href=\"crs_eco.htm#ECO230Y1\">ECO230Y1</a>, <a href=\"crs_eco.htm#ECO320H1\">ECO320H1</a>, <a href=\"crs_eco.htm#ECO324Y1\">ECO324Y1</a>, <a href=\"crs_eco.htm#ECO459H1\">ECO459H1</a>; <a href=\"crs_eng.htm#ENG270Y1\">ENG270Y1</a>, <a href=\"crs_eng.htm#ENG277Y1\">ENG277Y1</a>, <a href=\"crs_eng.htm#ENG278Y1\">ENG278Y1</a>, <a href=\"crs_eng.htm#ENG370H1\">ENG370H1</a>; <a href=\"crs_env.htm#ENV221H1\">ENV221H1</a>, <a href=\"crs_env.htm#ENV333H1\">ENV333H1</a>; <a href=\"crs_fre.htm#FCS291H1\">FCS291H1</a>, <a href=\"crs_fre.htm#FCS392H1\">FCS392H1</a>; <a href=\"crs_for.htm#FOR201H1\">FOR201H1</a>; <a href=\"crs_fre.htm#FRE332H1\">FRE332H1</a>, <a href=\"crs_fre.htm#FRE334H1\">FRE334H1</a>, <a href=\"crs_fre.htm#FRE336H1\">FRE336H1</a>; <a href=\"crs_ggr.htm#GGR338H1\">GGR338H1</a>, <a href=\"crs_ggr.htm#GGR419H1\">GGR419H1</a>; <a href=\"crs_ant.htm#HAJ453H1\">HAJ453H1</a>; <a href=\"crs_his.htm#HIS106Y1\">HIS106Y1</a>, <a href=\"crs_his.htm#HIS294Y1\">HIS294Y1</a>, <a href=\"crs_his.htm#HIS305H1\">HIS305H1</a>, <a href=\"crs_his.htm#HIS359H1\">HIS359H1</a>, <a href=\"crs_his.htm#HIS360H1\">HIS360H1</a>, <a href=\"crs_his.htm#HIS370H1\">HIS370H1</a>, <a href=\"crs_his.htm#HIS381H1\">HIS381H1</a>, <a href=\"crs_his.htm#HIS382H1\">HIS382H1</a>, <a href=\"crs_his.htm#HIS383H1\">HIS383H1</a>, <a href=\"crs_his.htm#HIS393H1\">HIS393H1</a>, <a href=\"crs_his.htm#HIS408Y1\">HIS408Y1</a>, <a href=\"crs_his.htm#HIS413H1\">HIS413H1</a>, <a href=\"crs_his.htm#HIS446H1\">HIS446H1</a>, <a href=\"crs_his.htm#HIS456Y1\">HIS456Y1</a>, <a href=\"crs_his.htm#HIS475H1\">HIS475H1</a>, <a href=\"crs_his.htm#HIS476H1\">HIS476H1</a>, <a href=\"crs_his.htm#HIS478H1\">HIS478H1</a>, <a href=\"crs_his.htm#HIS487H1\">HIS487H1</a>; <a href=\"crs_hmb.htm#HMB202H1\">HMB202H1</a>, <a href=\"crs_hmb.htm#HMB203H1\">HMB203H1</a>, <a href=\"crs_hmb.htm#HMB303H1\">HMB303H1</a>, <a href=\"crs_hmb.htm#HMB323H1\">HMB323H1</a>, <a href=\"crs_hmb.htm#HMB433H1\">HMB433H1</a>, <a href=\"crs_hmb.htm#HMB443H1\">HMB443H1</a>; <a href=\"crs_hps.htm#HPS375H1\">HPS375H1</a>, <a href=\"crs_hps.htm#HPS376H1\">HPS376H1</a>; <a href=\"crs_ini.htm#INI327Y1\">INI327Y1</a>, <a href=\"crs_ini.htm#INI380Y1\">INI380Y1</a>; JPR374Y1; <a href=\"crs_nfs.htm#NFS490H1\">NFS490H1</a>; <a href=\"crs_nmc.htm#NMC285H1\">NMC285H1</a>, <a href=\"crs_nmc.htm#NMC286H1\">NMC286H1</a>, <a href=\"crs_nmc.htm#NMC343H1\">NMC343H1</a>, <a href=\"crs_nmc.htm#NMC344H1\">NMC344H1</a>, <a href=\"crs_nmc.htm#NMC362Y1\">NMC362Y1</a>, <a href=\"crs_nmc.htm#NMC365Y1\">NMC365Y1</a>, <a href=\"crs_nmc.htm#NMC374H1\">NMC374H1</a>1, <a href=\"crs_nmc.htm#NMC376H1\">NMC376H1</a>, <a href=\"crs_nmc.htm#NMC377Y1\">NMC377Y1</a>, <a href=\"crs_nmc.htm#NMC378H1\">NMC378H1</a>, <a href=\"crs_nmc.htm#NMC381H1\">NMC381H1</a>, <a href=\"crs_nmc.htm#NMC393H1\">NMC393H1</a>; <a href=\"crs_new.htm#NEW223Y1\">NEW223Y1</a>, <a href=\"crs_new.htm#NEW224Y1\">NEW224Y1</a>, <a href=\"crs_new.htm#NEW324Y1\">NEW324Y1</a>, <a href=\"crs_new.htm#NEW325H1\">NEW325H1</a>, <a href=\"crs_new.htm#NEW326Y1\">NEW326Y1</a>; <a href=\"crs_phl.htm#PHL336H1\">PHL336H1</a>, <a href=\"crs_phl.htm#PHL380H1\">PHL380H1</a>; <a href=\"crs_pol.htm#POL201Y1\">POL201Y1</a>, <a href=\"crs_pol.htm#POL417Y1\">POL417Y1</a>, <a href=\"crs_pol.htm#POL447H1\">POL447H1</a>, <a href=\"crs_pol.htm#POL479H1\">POL479H1</a>, <a href=\"crs_pol.htm#POL482H1\">POL482H1</a>; <a href=\"crs_rlg.htm#RLG203Y1\">RLG203Y1</a>, <a href=\"crs_rlg.htm#RLG204Y1\">RLG204Y1</a>, <a href=\"crs_rlg.htm#RLG241Y1\">RLG241Y1</a>, <a href=\"crs_rlg.htm#RLG251H1\">RLG251H1</a>, <a href=\"crs_rlg.htm#RLG321H1\">RLG321H1</a>, <a href=\"crs_rlg.htm#RLG333H1\">RLG333H1</a>, <a href=\"crs_rlg.htm#RLG351H1\">RLG351H1</a>, <a href=\"crs_rlg.htm#RLG355H1\">RLG355H1</a>; <a href=\"crs_soc.htm#SOC210H1\">SOC210H1</a>; <a href=\"crs_wgs.htm#WGS330H1\">WGS330H1</a>, <a href=\"crs_wgs.htm#WGS369H1\">WGS369H1</a>, <a href=\"crs_wgs.htm#WGS380H1\">WGS380H1</a>, <a href=\"crs_wgs.htm#WGS440H1\">WGS440H1</a>, <a href=\"crs_wgs.htm#WGS463H1\">WGS463H1</a><br /><br /><strong>Group C:</strong><br />(<a href=\"crs_new.htm#NEW280Y1\">NEW280Y1</a>, <a href=\"crs_new.htm#NEW380Y1\">NEW380Y1</a>)/(<a href=\"crs_fre.htm#FSL221Y1\">FSL221Y1</a>, <a href=\"crs_fre.htm#FSL321Y1\">FSL321Y1</a>/ <a href=\"crs_fre.htm#FSL421Y1\">FSL421Y1</a>)/(<a href=\"crs_nmc.htm#NML110Y1\">NML110Y1</a>, <a href=\"crs_nmc.htm#NML211Y1\">NML211Y1</a>)/(<a href=\"crs_prt.htm#PRT100Y1\">PRT100Y1</a>/ <a href=\"crs_prt.htm#PRT110Y1\">PRT110Y1</a>, <a href=\"crs_prt.htm#PRT220Y1\">PRT220Y1</a>); or two courses in a major African language approved by the Program Committee<br />";
                        }
                        content = HttpUtility.HtmlDecode(content);
                        content = BrRegex.Replace(content, "|");
                        content = content.Replace("</p>", "|");
                        content = content.Replace("</li>", "|");
                        content = content.Replace("</div>", "|");
                        content = content.Replace("/", " / ");

                        content = AngleRegex.Replace(content, String.Empty);

                        SchoolProgram program = new SchoolProgram()
                        {
                            Name = formatter.ToTitleCase(name).Trim(' '),
                            Description = content,
                            Campus = "UTSG"
                        };

                        Console.WriteLine("Program: " + name);

                        results.Add(program);
                    }
                }
            }
            return results;
        }
    }
}
