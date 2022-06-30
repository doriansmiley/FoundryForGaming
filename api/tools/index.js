#!/usr/bin/env node

import {Command} from "commander";
import sendTestEvents from "./send-events.js";

const cli = new Command();

cli.description("Access the JSON Placeholder API");
cli.name("jsonp");
cli.usage("<command>");
cli.addHelpCommand(false);
cli.helpOption(false);

cli
  .command("post-events")
  .argument("[count]", "Number of events to send. Must be a number, greater than zero, and less than 1000. Default to 1", 1)
  .option("-p, --pretty", "Pretty-print output from the API.")
  .description(
    "Post test events to the API"
  )
  .action(sendTestEvents);

cli.parse(process.argv);
