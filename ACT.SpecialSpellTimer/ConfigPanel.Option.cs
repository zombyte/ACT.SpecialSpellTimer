﻿namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// Configパネル オプション
    /// </summary>
    public partial class ConfigPanel
    {
        /// <summary>
        /// オプションのLoad
        /// </summary>
        private void LoadOption()
        {
            this.LoadSettingsOption();

            this.OverlayForceVisibleCheckBox.CheckedChanged += (s1, e1) =>
            {
                Settings.Default.OverlayForceVisible = this.OverlayForceVisibleCheckBox.Checked;
                Settings.Default.Save();
            };

            this.SwitchOverlayButton.Click += (s1, e1) =>
            {
                Settings.Default.OverlayVisible = !Settings.Default.OverlayVisible;
                Settings.Default.Save();
                this.LoadSettingsOption();

                if (Settings.Default.OverlayVisible)
                {
                    SpellTimerCore.Default.ActivatePanels();
                    OnePointTelopController.ActivateTelops();
                }
            };

            this.SwitchTelopButton.Click += (s1, e1) =>
            {
                Settings.Default.TelopAlwaysVisible = !Settings.Default.TelopAlwaysVisible;
                Settings.Default.Save();
                this.LoadSettingsOption();
            };

            this.LanguageComboBox.SelectedValueChanged += (s1, e1) =>
            {
                Language language = (Language)this.LanguageComboBox.SelectedItem;
                this.LanguageRestartLabel.Text = Utility.Translate.GetTranslationsFor(language.Value).GetString("RequiresRestart");
                Settings.Default.Language = language.Value;
                Settings.Default.Save();
                this.LoadSettingsOption();
            };

            Action action = new Action(() =>
            {
                if (Settings.Default.OverlayVisible)
                {
                    this.SwitchOverlayButton.Text = Translate.Get("OverlayDisplaySwitchIsOn");
                }
                else
                {
                    this.SwitchOverlayButton.Text = Translate.Get("OverlayDisplaySwitchIsOff");
                }
            });

            this.OptionTabPage.MouseHover += (s1, e1) => action();
            this.SwitchOverlayButton.MouseHover += (s1, e1) => action();
        }

        /// <summary>
        /// 初期化 Button
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ShokikaButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this,
                Translate.Get("ResetAllPrompt"),
                "ACT.SpecialSpellTimer",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }

            Settings.Default.Reset();
            Settings.Default.Save();

            PanelSettings.Default.SettingsTable.Clear();
            PanelSettings.Default.Save();

            foreach (var telop in OnePointTelopTable.Default.Table)
            {
                telop.Left = 10.0d;
                telop.Top = 10.0d;
            }

            OnePointTelopTable.Default.Save();

            this.LoadSettingsOption();
            SpellTimerCore.Default.LayoutPanels();
        }

        /// <summary>
        /// 適用する Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TekiyoButton_Click(object sender, EventArgs e)
        {
            this.ApplySettingsOption();

            // Windowを一旦すべて閉じる
            SpellTimerCore.Default.ClosePanels();
            OnePointTelopController.CloseTelops();
        }

        /// <summary>
        /// オプション設定をロードする
        /// </summary>
        private void LoadSettingsOption()
        {
            foreach (Language lang in this.LanguageComboBox.Items)
            {
                if (lang.Value == Settings.Default.Language)
                    this.LanguageComboBox.SelectedItem = lang;
            }

            this.OverlayForceVisibleCheckBox.Checked = Settings.Default.OverlayForceVisible;

            if (Settings.Default.OverlayVisible)
            {
                this.SwitchOverlayButton.Text = Translate.Get("OverlayDisplaySwitchIsOn");
            }
            else
            {
                this.SwitchOverlayButton.Text = Translate.Get("OverlayDisplaySwitchIsOff");
            }

            if (Settings.Default.TelopAlwaysVisible)
            {
                this.SwitchTelopButton.Text = Translate.Get("TelopDisplaySwitchIsOn");
            }
            else
            {
                this.SwitchTelopButton.Text = Translate.Get("TelopDisplaySwitchIsOff");
            }

            this.DefaultVisualSetting.BarSize = Settings.Default.ProgressBarSize;
            this.DefaultVisualSetting.BarColor = Settings.Default.ProgressBarColor;
            this.DefaultVisualSetting.BarOutlineColor = Settings.Default.ProgressBarOutlineColor;
            this.DefaultVisualSetting.TextFont = Settings.Default.Font;
            this.DefaultVisualSetting.FontColor = Settings.Default.FontColor;
            this.DefaultVisualSetting.FontOutlineColor = Settings.Default.FontOutlineColor;
            this.DefaultVisualSetting.BackgroundColor = Settings.Default.BackgroundColor;
            this.DefaultVisualSetting.RefreshSampleImage();
            
            this.OpacityNumericUpDown.Value = Settings.Default.Opacity;
            this.ClickThroughCheckBox.Checked = Settings.Default.ClickThroughEnabled;
            this.AutoSortCheckBox.Checked = Settings.Default.AutoSortEnabled;
            this.AutoSortReverseCheckBox.Checked = Settings.Default.AutoSortReverse;
            this.TimeOfHideNumericUpDown.Value = (decimal)Settings.Default.TimeOfHideSpell;
            this.RefreshIntervalNumericUpDown.Value = Settings.Default.RefreshInterval;
            this.EnabledPTPlaceholderCheckBox.Checked = Settings.Default.EnabledPartyMemberPlaceholder;
        }

        /// <summary>
        /// 設定を適用する
        /// </summary>
        private void ApplySettingsOption()
        {
            Settings.Default.Language = ((Utility.Language)this.LanguageComboBox.SelectedItem).Value;
            Settings.Default.OverlayForceVisible = this.OverlayForceVisibleCheckBox.Checked;
            Settings.Default.ProgressBarSize = this.DefaultVisualSetting.BarSize;
            Settings.Default.ProgressBarColor = this.DefaultVisualSetting.BarColor;
            Settings.Default.ProgressBarOutlineColor = this.DefaultVisualSetting.BarOutlineColor;
            Settings.Default.Font = this.DefaultVisualSetting.TextFont;
            Settings.Default.FontColor = this.DefaultVisualSetting.FontColor;
            Settings.Default.FontOutlineColor = this.DefaultVisualSetting.FontOutlineColor;
            Settings.Default.BackgroundColor = this.DefaultVisualSetting.BackgroundColor;

            Settings.Default.Opacity = (int)this.OpacityNumericUpDown.Value;
            Settings.Default.ClickThroughEnabled = this.ClickThroughCheckBox.Checked;
            Settings.Default.AutoSortEnabled = this.AutoSortCheckBox.Checked;
            Settings.Default.AutoSortReverse = this.AutoSortReverseCheckBox.Checked;
            Settings.Default.TimeOfHideSpell = (double)this.TimeOfHideNumericUpDown.Value;
            Settings.Default.RefreshInterval = (long)this.RefreshIntervalNumericUpDown.Value;
            Settings.Default.EnabledPartyMemberPlaceholder = this.EnabledPTPlaceholderCheckBox.Checked;

            // 設定を保存する
            Settings.Default.Save();
        }
    }
}
