using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konohagakure.VillagerApplication
{
	public class ButtonCommandVillagerApplication : ButtonCommandModule
	{
		[ButtonCommand("btn_CreateApplication")]
		public async Task CreateVillagerApplication(ButtonContext ctx)
		{
			var modalVillagerApplication = ModalBuilder.Create("VillagerApplication")
				.WithTitle("Konohagakure — Villager Application")
				.AddComponents(new TextInputComponent("IGN:", "ingamenameTextBox", "Name of Character", null, true, TextInputStyle.Short))
				.AddComponents(new TextInputComponent("Introduction:", "introductionTextBox", "Introduce youself & your reason for joining", null, true, TextInputStyle.Paragraph))
				.AddComponents(new TextInputComponent("Alt(s):", "altsTextBox", "IGN, Alt1, Alt2, ... \nPlease be sure to separate your alt(s) with a comma!", null, true, TextInputStyle.Paragraph));

			await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalVillagerApplication);
		}
	}
}
