using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cu_grab
{
    namespace Shows
    {
        namespace DPlay
        {
            public class ImageMeta
            {
                public int aperture { get; set; }
                public string credit { get; set; }
                public string camera { get; set; }
                public string caption { get; set; }
                public int created_timestamp { get; set; }
                public string copyright { get; set; }
                public int focal_length { get; set; }
                public int iso { get; set; }
                public int shutter_speed { get; set; }
                public string title { get; set; }
            }

            public class Crop
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image16x9
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop2
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image3x2
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop2 crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop3
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image1x1
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop3 crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop4
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image2x1
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop4 crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop5
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image16x10
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop5 crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop6
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image4x1
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop6 crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop7
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image3x1
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop7 crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop8
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image16x17
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop8 crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop9
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image352x259
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop9 crop { get; set; }
                public int linked { get; set; }
            }

            public class Crop10
            {
                public int x { get; set; }
                public int y { get; set; }
                public int w { get; set; }
                public int h { get; set; }
            }

            public class Image16x7
            {
                public string file { get; set; }
                public int width { get; set; }
                public int height { get; set; }
                public Crop10 crop { get; set; }
                public int linked { get; set; }
            }

            public class Sizes
            {
                public Image16x9 image16x9 { get; set; }
                public Image3x2 image3x2 { get; set; }
                public Image1x1 image1x1 { get; set; }
                public Image2x1 image2x1 { get; set; }
                public Image16x10 image16x10 { get; set; }
                public Image4x1 image4x1 { get; set; }
                public Image3x1 image3x1 { get; set; }
                public Image16x17 image16x17 { get; set; }
                public Image352x259 image352x259 { get; set; }
                public Image16x7 image16x7 { get; set; }
            }

            public class ImageData
            {
                public int width { get; set; }
                public int height { get; set; }
                public string file { get; set; }
                public ImageMeta image_meta { get; set; }
                public Sizes sizes { get; set; }
                public string type { get; set; }
                public string alt { get; set; }
            }

            public class Node
            {
                public int term_id { get; set; }
                public string name { get; set; }
                public string slug { get; set; }
                public int term_group { get; set; }
                public int term_taxonomy_id { get; set; }
                public string taxonomy { get; set; }
                public string description { get; set; }
                public int parent { get; set; }
                public int count { get; set; }
                public string filter { get; set; }
            }

            public class Metadata
            {
                public string type { get; set; }
                public string displayAs { get; set; }
                public string ssid { get; set; }
                public string ssidOverride { get; set; }
                public string tags { get; set; }
                public string seasons_total { get; set; }
                public string brightcove_show_id { get; set; }
                public string brightcove_genre_id { get; set; }
                public string potential { get; set; }
                public string nav_menu { get; set; }
                public string onairday { get; set; }
                public string onairtime { get; set; }
                public string onairsuffix { get; set; }
                public string facebooklink { get; set; }
                public string twitterlink { get; set; }
                public string twitterhashtag { get; set; }
                public string episodes { get; set; }
                public string tvlisting_show_series_id { get; set; }
                public string image_id { get; set; }
                public object image_data { get; set; }
                public string poster_image_logo_id { get; set; }
                public string flash_player_logo_id { get; set; }
                public string html_player_logo_id { get; set; }
                public string video_page_id { get; set; }
                public string trailer_page_id { get; set; }
                public string season_number { get; set; }
                public string episode_number { get; set; }
                public string video_page_asset_id { get; set; }
                public string trailer_page_asset_id { get; set; }
                public bool poster_image_logo_data { get; set; }
                public bool flash_player_logo_data { get; set; }
                public bool html_player_logo_data { get; set; }
                public string ordering { get; set; }
                public string about_link { get; set; }
                public string color { get; set; }
                public string level { get; set; }
                public string make_labels_visible { get; set; }
            }

            public class TaxonomyItem
            {
                public int term_id { get; set; }
                public string name { get; set; }
                public string slug { get; set; }
                public string description { get; set; }
                public int parent { get; set; }
                public int count { get; set; }
                public string filter { get; set; }
                public string type { get; set; }
                public Node node { get; set; }
                public Metadata metadata { get; set; }
            }

            public class PackageLabel
            {
                public bool enabled { get; set; }
                public string value { get; set; }
                public string valueShort { get; set; }
                public string color { get; set; }
            }

            public class LiveLabel
            {
                public bool enabled { get; set; }
                public string value { get; set; }
                public string valueShort { get; set; }
                public string color { get; set; }
            }

            public class ContentInfo
            {
                public PackageLabel package_label { get; set; }
                public LiveLabel live_label { get; set; }
            }

            public class Datum
            {
                public string id { get; set; }
                public string post_type { get; set; }
                public string type { get; set; }
                public string status { get; set; }
                public string title { get; set; }
                public string url { get; set; }
                public string created { get; set; }
                public string modified { get; set; }
                public string secondary_title { get; set; }
                public string caption { get; set; }
                public string description { get; set; }
                public string image_id { get; set; }
                public ImageData image_data { get; set; }
                public string image_meta { get; set; }
                public string tags { get; set; }
                public object bdat_tags { get; set; }
                public object video_metadata_genres { get; set; }
                public object prominence { get; set; }
                public string sort_title { get; set; }
                public string sponsor { get; set; }
                public string video_metadata_name { get; set; }
                public string video_metadata_tags { get; set; }
                public string video_metadata_cloudinary_still_image_id { get; set; }
                public string video_metadata_videoStillURL { get; set; }
                public string video_metadata_longDescription { get; set; }
                public int video_metadata_season { get; set; }
                public int video_metadata_episode { get; set; }
                public int season { get; set; }
                public int episode { get; set; }
                public string video_metadata_length { get; set; }
                public string video_metadata_tx_date { get; set; }
                public object video_metadata_tx_date_end { get; set; }
                public string video_metadata_genre { get; set; }
                public int video_metadata_first_startTime { get; set; }
                public bool video_metadata_first_run_endTime { get; set; }
                public int video_metadata_svod_preview { get; set; }
                public int video_metadata_svod_start_time { get; set; }
                public int video_metadata_svod_end_time { get; set; }
                public int video_metadata_advod_start_time { get; set; }
                public int video_metadata_advod_end_time { get; set; }
                public string video_advod_start_time { get; set; }
                public string video_advod_end_time { get; set; }
                public string video_metadata_provider { get; set; }
                public object video_metadata_referenceid { get; set; }
                public string hds { get; set; }
                public string hls { get; set; }
                public string view_cuepoints { get; set; }
                public object video_metadata_mediaid { get; set; }
                public int video_unpublish_date { get; set; }
                public int video_publish_date { get; set; }
                public string video_metadata_show { get; set; }
                public object video_metadata_type { get; set; }
                public string video_metadata_package { get; set; }
                public int popularity { get; set; }
                public string video_metadata_homeChannel { get; set; }
                public string source_system_name { get; set; }
                public string video_metadata_id { get; set; }
                public object latest_episode_content_id { get; set; }
                public object video_published_date { get; set; }
                public List<TaxonomyItem> taxonomy_items { get; set; }
                public ContentInfo content_info { get; set; }
                public string video_metadata_backup_video_asset_paths_hls { get; set; }
                public string video_metadata_backup_video_asset_paths_hds { get; set; }
                public string video_metadata_drmid_playready { get; set; }
                public string video_metadata_drmid_flashaccess { get; set; }
                public string video_metadata_streamtype { get; set; }
                public string video_metadata_analyticsid { get; set; }
                public string video_metadata_rendertemplate { get; set; }
                public string show_metadata_rendertemplate { get; set; }
                public string channel_metadata_source_system_id { get; set; }
                public string channel_metadata_analytics_channel_id { get; set; }
                public int minimum_age { get; set; }
                public bool simulcast { get; set; }
                public object simulcast_info { get; set; }
                public object page_settings { get; set; }
                public object link_title { get; set; }
                public override string ToString()
                {
                    return title;
                }
            }

            public class ShowsDPlay
            {
                public string status { get; set; }
                public int page { get; set; }
                public int items { get; set; }
                public int total_pages { get; set; }
                public int total_items { get; set; }
                public List<object> filters { get; set; }
                public List<Datum> data { get; set; }
            }

        }
    }
}