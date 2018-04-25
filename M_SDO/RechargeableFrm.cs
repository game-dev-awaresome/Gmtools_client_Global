using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using C_Global;
using C_Event;
using Language;
namespace M_SDO
{
    /// <summary>
    /// 
    /// </summary>
    [C_Global.CModuleAttribute("玩家消费信息", "Frm_SDO_Rechargeable", "SDO管理工具 -- 玩家消费信息", "SDO Group")]  
    public partial class Frm_SDO_Rechargeable : Form
    {
        private CEnum.Message_Body[,] mServerInfo = null;
        private CSocketEvent m_ClientEvent = null;
        private CSocketEvent tmp_ClientEvent = null;
        private int iPageCount = 0;
        private bool bFirst = false;

        public Frm_SDO_Rechargeable()
        {
            InitializeComponent();
        }

        #region 自定义调用事件
        /// <summary>
        /// 创建类库中的窗体
        /// </summary>
        /// <param name="oParent">MDI 程序的父窗体</param>
        /// <param name="oSocket">Socket</param>
        /// <returns>类库中的窗体</returns>
        public Form CreateModule(object oParent, object oEvent)
        {
            //创建登录窗体
            Frm_SDO_Rechargeable mModuleFrm = new Frm_SDO_Rechargeable();
            mModuleFrm.m_ClientEvent = (CSocketEvent)oEvent;
            if (oParent != null)
            {
                mModuleFrm.MdiParent = (Form)oParent;
                mModuleFrm.Show();
            }
            else
            {
                mModuleFrm.ShowDialog();
            }

            return mModuleFrm;
        }
        #endregion
        #region 语言库
        /// <summary>
        ///　文字库
        /// </summary>
        ConfigValue config = null;

        /// <summary>
        /// 初始化华文字语言库
        /// </summary>
        private void IntiFontLib()
        {
            config = (ConfigValue)m_ClientEvent.GetInfo("INI");
            IntiUI();
        }

        private void IntiUI()
        {
            this.Text = config.ReadConfigValue("MSDO", "RG_UI_SDORechargeable");
            this.GrpSearch.Text = config.ReadConfigValue("MSDO", "RG_UI_GrpSearch");
            this.LblServer.Text = config.ReadConfigValue("MSDO", "RG_UI_LblServer");
            this.LblAccount.Text = config.ReadConfigValue("MSDO", "RG_UI_LblAccount");
            this.LblDate.Text = config.ReadConfigValue("MSDO", "RG_UI_LblDate");
            this.LblLink.Text = config.ReadConfigValue("MSDO", "RG_UI_LblLink");
            this.BtnSearch.Text = config.ReadConfigValue("MSDO", "RG_UI_BtnSearch");
            this.BtnClose.Text = config.ReadConfigValue("MSDO", "RG_UI_BtnClose");
            this.tabPage1.Text = config.ReadConfigValue("MSDO", "RG_UI_GrpResult");
            this.tabPage2.Text = config.ReadConfigValue("MSDO", "RG_UI_GrpResult1");
            this.LblPage.Text = config.ReadConfigValue("MSDO", "RG_UI_LblPage");
        }


                #endregion
        private void Frm_SDO_Rechargeable_Load(object sender, EventArgs e)
        {
            //服务器列表
            IntiFontLib();
            CEnum.Message_Body[] mContent = new CEnum.Message_Body[2];
            mContent[0].eName = CEnum.TagName.ServerInfo_GameDBID;
            mContent[0].eTag = CEnum.TagFormat.TLV_INTEGER;
            mContent[0].oContent = 3;

            mContent[1].eName = CEnum.TagName.ServerInfo_GameID;
            mContent[1].eTag = CEnum.TagFormat.TLV_INTEGER;
            mContent[1].oContent = m_ClientEvent.GetInfo("GameID_SDO");

            this.backgroundWorkerFormLoad.RunWorkerAsync(mContent);

            //mServerInfo = Operation_SDO.GetServerList(this.m_ClientEvent, mContent);

            //CmbServer = Operation_SDO.BuildCombox(mServerInfo, CmbServer);

            //PnlPage.Visible = false;
            //LblTotal.Text = "";
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (CmbServer.Text == "" || this.backgroundWorkerPageChanged.IsBusy)
            {
                return;
            }
            PnlPage.Visible = false;


            if (TxtAccount.Text.Trim().Length > 0)
            {
                if (this.TabQueryType.SelectedTab.Text == config.ReadConfigValue("MSDO", "RG_UI_GrpResult"))
                {
                    this.BtnSearch.Enabled = false;
                    this.Cursor = Cursors.AppStarting;
                    GrdResult.DataSource = null;

                    CEnum.Message_Body[] mContent = new CEnum.Message_Body[6];

                    mContent[0].eName = CEnum.TagName.SDO_Account;
                    mContent[0].eTag = CEnum.TagFormat.TLV_STRING;
                    mContent[0].oContent = TxtAccount.Text;

                    mContent[1].eName = CEnum.TagName.SDO_ServerIP;
                    mContent[1].eTag = CEnum.TagFormat.TLV_STRING;
                    mContent[1].oContent = Operation_SDO.GetItemAddr(mServerInfo, CmbServer.Text);

                    mContent[2].eName = CEnum.TagName.SDO_BeginTime;
                    mContent[2].eTag = CEnum.TagFormat.TLV_DATE;
                    mContent[2].oContent = DtpBegin.Value;

                    mContent[3].eName = CEnum.TagName.SDO_EndTime;
                    mContent[3].eTag = CEnum.TagFormat.TLV_DATE;
                    mContent[3].oContent = DtpEnd.Value;

                    mContent[4].eName = CEnum.TagName.Index;
                    mContent[4].eTag = CEnum.TagFormat.TLV_INTEGER;
                    mContent[4].oContent = 1;

                    mContent[5].eName = CEnum.TagName.PageSize;
                    mContent[5].eTag = CEnum.TagFormat.TLV_INTEGER;
                    mContent[5].oContent = Operation_SDO.iPageSize;

                    ArrayList paramList = new ArrayList();
                    paramList.Add(mContent);


                    //合计
                    mContent = new CEnum.Message_Body[4];

                    mContent[0].eName = CEnum.TagName.SDO_Account;
                    mContent[0].eTag = CEnum.TagFormat.TLV_STRING;
                    mContent[0].oContent = TxtAccount.Text;

                    mContent[1].eName = CEnum.TagName.SDO_ServerIP;
                    mContent[1].eTag = CEnum.TagFormat.TLV_STRING;
                    mContent[1].oContent = Operation_SDO.GetItemAddr(mServerInfo, CmbServer.Text);

                    mContent[2].eName = CEnum.TagName.SDO_BeginTime;
                    mContent[2].eTag = CEnum.TagFormat.TLV_DATE;
                    mContent[2].oContent = DtpBegin.Value;

                    mContent[3].eName = CEnum.TagName.SDO_EndTime;
                    mContent[3].eTag = CEnum.TagFormat.TLV_DATE;
                    mContent[3].oContent = DtpEnd.Value;

                    paramList.Add(mContent);
                    this.backgroundWorkerSearch.RunWorkerAsync(paramList);
                }
                else
                {
                    this.BtnSearch.Enabled = false;
                    this.Cursor = Cursors.AppStarting;
                    DataGrd.DataSource = null;

                    CEnum.Message_Body[] mContent = new CEnum.Message_Body[6];

                    mContent[0].eName = CEnum.TagName.SDO_Account;
                    mContent[0].eTag = CEnum.TagFormat.TLV_STRING;
                    mContent[0].oContent = TxtAccount.Text;

                    mContent[1].eName = CEnum.TagName.SDO_ServerIP;
                    mContent[1].eTag = CEnum.TagFormat.TLV_STRING;
                    mContent[1].oContent = Operation_SDO.GetItemAddr(mServerInfo, CmbServer.Text);

                    mContent[2].eName = CEnum.TagName.SDO_BeginTime;
                    mContent[2].eTag = CEnum.TagFormat.TLV_DATE;
                    mContent[2].oContent = DtpBegin.Value;

                    mContent[3].eName = CEnum.TagName.SDO_EndTime;
                    mContent[3].eTag = CEnum.TagFormat.TLV_DATE;
                    mContent[3].oContent = DtpEnd.Value;

                    mContent[4].eName = CEnum.TagName.Index;
                    mContent[4].eTag = CEnum.TagFormat.TLV_INTEGER;
                    mContent[4].oContent = 1;

                    mContent[5].eName = CEnum.TagName.PageSize;
                    mContent[5].eTag = CEnum.TagFormat.TLV_INTEGER;
                    mContent[5].oContent = Operation_SDO.iPageSize;

                    backgroundWorkerSearchNotM.RunWorkerAsync(mContent);
                }
            }
            else
            {
                MessageBox.Show(config.ReadConfigValue("MSDO", "RG_Code_Msg"));
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CmbPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bFirst)
            {
                CmbPage.Enabled = false;
                Cursor = Cursors.AppStarting;
                CEnum.Message_Body[] mContent = new CEnum.Message_Body[6];

                mContent[0].eName = CEnum.TagName.SDO_Account;
                mContent[0].eTag = CEnum.TagFormat.TLV_STRING;
                mContent[0].oContent = TxtAccount.Text;

                mContent[1].eName = CEnum.TagName.SDO_ServerIP;
                mContent[1].eTag = CEnum.TagFormat.TLV_STRING;
                mContent[1].oContent = Operation_SDO.GetItemAddr(mServerInfo, CmbServer.Text);

                mContent[2].eName = CEnum.TagName.SDO_BeginTime;
                mContent[2].eTag = CEnum.TagFormat.TLV_DATE;
                mContent[2].oContent = DtpBegin.Value;

                mContent[3].eName = CEnum.TagName.SDO_EndTime;
                mContent[3].eTag = CEnum.TagFormat.TLV_DATE;
                mContent[3].oContent = DtpEnd.Value;

                mContent[4].eName = CEnum.TagName.Index;
                mContent[4].eTag = CEnum.TagFormat.TLV_INTEGER;
                mContent[4].oContent = (int.Parse(CmbPage.Text) - 1) * Operation_SDO.iPageSize + 1; ;

                mContent[5].eName = CEnum.TagName.PageSize;
                mContent[5].eTag = CEnum.TagFormat.TLV_INTEGER;
                mContent[5].oContent = Operation_SDO.iPageSize;

                this.backgroundWorkerPageChanged.RunWorkerAsync(mContent);

                //CEnum.Message_Body[,] mResult = Operation_SDO.GetResult(m_ClientEvent.GetSocket(m_ClientEvent,Operation_SDO.GetItemAddr(mServerInfo, CmbServer.Text)), CEnum.ServiceKey.SDO_USERMCASH_QUERY, mContent);

                //Operation_SDO.BuildDataTable(this.m_ClientEvent, mResult, GrdResult, out iPageCount);
            }
        }

        private void backgroundWorkerFormLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (typeof(C_Event.CSocketEvent))
            {
                mServerInfo = Operation_SDO.GetServerList(this.m_ClientEvent, (CEnum.Message_Body[])e.Argument);
            }
        }

        private void backgroundWorkerFormLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CmbServer = Operation_SDO.BuildCombox(mServerInfo, CmbServer);

            PnlPage.Visible = false;
            LblTotal.Text = "";
            tmp_ClientEvent = m_ClientEvent.GetSocket(m_ClientEvent, Operation_SDO.GetItemAddr(mServerInfo, CmbServer.Text));
        }

        private void CmbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            tmp_ClientEvent = m_ClientEvent.GetSocket(m_ClientEvent, Operation_SDO.GetItemAddr(mServerInfo, CmbServer.Text));
        }

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            ArrayList paramList = (ArrayList)e.Argument;
            ArrayList resultList = new ArrayList();
            lock (typeof(C_Event.CSocketEvent))
            {
                CEnum.Message_Body[,] mResult = Operation_SDO.GetResult(tmp_ClientEvent, CEnum.ServiceKey.SDO_USERMCASH_QUERY, (CEnum.Message_Body[])paramList[0]);
                if (mResult[0, 0].eName == CEnum.TagName.ERROR_Msg)
                {
                    e.Cancel = true;
                    MessageBox.Show(mResult[0, 0].oContent.ToString());
                    return;
                }
                resultList.Add(mResult);
                mResult = Operation_SDO.GetResult(tmp_ClientEvent, CEnum.ServiceKey.SDO_USERCHARAGESUM_QUERY, (CEnum.Message_Body[])paramList[1]);
                if (mResult[0, 0].eName == CEnum.TagName.ERROR_Msg)
                {
                    e.Cancel = true;
                    MessageBox.Show(mResult[0, 0].oContent.ToString());
                    return;
                }
                resultList.Add(mResult);

            }
            e.Result = resultList;
        }

        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.BtnSearch.Enabled = true;
            this.Cursor = Cursors.Default;
            if (!e.Cancelled)
            {
                CEnum.Message_Body[,] mResult = (CEnum.Message_Body[,])(((ArrayList)e.Result)[0]);
                if (mResult[0, 0].eName == CEnum.TagName.ERROR_Msg)
                {
                    MessageBox.Show(mResult[0, 0].oContent.ToString());
                    return;
                }

                Operation_SDO.BuildDataTable(this.m_ClientEvent, mResult, GrdResult, out iPageCount);

                if (iPageCount <= 0)
                {
                    PnlPage.Visible = false;
                }
                else
                {
                    for (int i = 0; i < iPageCount; i++)
                    {
                        CmbPage.Items.Add(i + 1);
                    }

                    CmbPage.SelectedIndex = 0;
                    bFirst = true;
                    PnlPage.Visible = true;
                }

                mResult = (CEnum.Message_Body[,])(((ArrayList)e.Result)[1]);

                if (mResult[0, 0].eName == CEnum.TagName.ERROR_Msg)
                {
                    return;
                }

                LblTotal.Text = config.ReadConfigValue("MSDO", "RG_Code_Sum") + mResult[0, 0].oContent.ToString();
            }
            //else
            //{
            //    MessageBox.Show(e.Result.ToString());
            //}
        }

        private void backgroundWorkerPageChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (typeof(C_Event.CSocketEvent))
            {
                e.Result = Operation_SDO.GetResult(tmp_ClientEvent, CEnum.ServiceKey.SDO_USERMCASH_QUERY, (CEnum.Message_Body[])e.Argument);
            }
        }

        private void backgroundWorkerPageChanged_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CmbPage.Enabled = true;
            Cursor = Cursors.Default;
            CEnum.Message_Body[,] mResult = (CEnum.Message_Body[,])e.Result;

            Operation_SDO.BuildDataTable(this.m_ClientEvent, mResult, GrdResult, out iPageCount);
        }

        private void backgroundWorkerSearchNotM_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (typeof(C_Event.CSocketEvent))
            {
                e.Result = Operation_SDO.GetResult(tmp_ClientEvent, CEnum.ServiceKey.SDO_NotReachMoney_Query, (CEnum.Message_Body[])e.Argument);
            }
        }

        private void backgroundWorkerSearchNotM_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BtnSearch.Enabled = true;
            Cursor = Cursors.Default;
            CEnum.Message_Body[,] mResult = (CEnum.Message_Body[,])e.Result;
            if (mResult[0, 0].eName == CEnum.TagName.ERROR_Msg)
            {
                MessageBox.Show(mResult[0, 0].oContent.ToString());
                return;
            }
            Operation_SDO.BuildDataTable(this.m_ClientEvent, mResult, DataGrd, out iPageCount);
        }


    }
}