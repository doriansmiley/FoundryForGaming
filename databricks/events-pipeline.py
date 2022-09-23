# Databricks notebook source
import dlt
from pyspark.sql.functions import *
from pyspark.sql.types import *

spark.sparkContext._jsc.hadoopConfiguration().set("fs.s3a.access.key", spark.conf.get("awsPublic"))
spark.sparkContext._jsc.hadoopConfiguration().set("fs.s3a.secret.key", spark.conf.get("awsPrivate"))

# If you are using Auto Loader file notification mode to load files, provide the AWS Region ID.
aws_region = "us-west-2"
spark.sparkContext._jsc.hadoopConfiguration().set("fs.s3a.endpoint", "s3." + aws_region + ".amazonaws.com")

json_path = "s3a://foundry-for-gaming/2022/6/31/23/"
@dlt.table(
  comment="The FFG clickstream dataset, ingested from S3"
)
def events_raw():
  return (spark.read.format("json").option("multiline", "true").load(json_path))

@dlt.table(
  comment="Events clickstream data cleaned and prepared for analysis."
)
@dlt.expect_or_fail("uid", "version IS NOT NULL")
def events_prepared():
  return (
    dlt.read("events_raw")
      .select(
        col('uid').alias('uid'),
        from_unixtime(col('ts')/1000, 'yyyy-MM-dd HH:mm:ss').alias('ts').cast('timestamp'),
        col('sessionId').alias('sessionId'),
        col('version').alias('version'),
        col('platform').alias('platform'),
        col('device').alias('device'),
        col('client').alias('client'),
        col('deviceID').alias('deviceID'),
        col('ip').alias('ip'),
        col('countryName').alias('countryName'),
        col('actor').alias('actor'),

        col('demographics.age').alias('age'),
        col('demographics.sex').alias('sex'),
        col('demographics.language').alias('language'),

        col('location.coordinates').getItem(0).alias('lat'),
        col('location.coordinates').getItem(1).alias('lng'),

        col('evtAttributes.screen').alias('screen'),
        col('evtAttributes.element').alias('element'),
        col('evtAttributes.label').alias('label'),
        col('evtAttributes.interaction').alias('interaction'),
        col('evtAttributes.value').alias('value'),
        col('evtAttributes.variant.id').alias('element_variant_id'),
        col('evtAttributes.variant.value').alias('element_variant_value'),
        col('evtAttributes.activity.type').alias('activity_type'),
        col('evtAttributes.activity.experiment.id').alias('activity_variant_id'),
        col('evtAttributes.activity.experiment.value').alias('activity_variant_value'),
        col('evtAttributes.screenInfo.width').alias('screen_info_width'),
        col('evtAttributes.screenInfo.height').alias('screen_info_height'),
        col('evtAttributes.screenInfo.fps').alias('screen_info_fps'),
    )
  )

@dlt.table(
  comment="A table containing the clean events data."
)
def events():
  return (
    dlt.read("events_prepared")
  )
