<template>
  <div :class="['post', {'mat-hidden': post.isHidden}, {'mat-deleted': post.isDeleted}]">
    <q-item :to="to" class="header page-padding">
      <q-avatar class="shadow-1 avatar" size="44px">
        <img :src="$imagePath(post.authorAvatar)"/>
      </q-avatar>
      <div>
        <div class="blog-title my-header">
          <q-icon name="fas fa-trash" color="maroon" class="q-mr-sm" v-if="post.isDeleted"/>
          <q-icon name="far fa-eye-slash" v-else-if="post.isHidden" class="q-mr-sm"/>
          {{post.title}}
          <span class="q-ml-sm" v-if="post.isDeleted">
            [{{$tl("deleted")}}]
          </span>
          <span class="q-ml-sm" v-else-if="post.isHidden">
            [{{$tl("hidden")}}]
          </span>
        </div>
        <div>
          <router-link :to="{name: 'User', params: {link: post.authorLink}}" class="user-link">
            {{post.authorName}}
          </router-link>
        </div>
      </div>
    </q-item>

    <div v-if="!post.isHidden && !post.isDeleted" class="post-text page-padding"
         v-html="post.preview"></div>

    <div class="date text-grey-6">
      <q-icon name="far fa-clock"/>
      <span>{{$formatDate(this.post.publishDate)}} &nbsp;</span>
    </div>

    <div class="flex footer float-left ">
      <q-item class="page-padding-left" :to="toComments">
        <span :class="[{'text-grey-6': !post.commentsCount}]">
        <q-icon name="far fa-comment" class="q-mr-sm"/>
        {{post.commentsCount}} {{$tl('commentsCount')}}
        </span>
      </q-item>
      <q-item :to="to" v-if="post.hasMoreText">
        <span>
          {{$tl('readMore')}}
          <q-icon name="fas fa-arrow-right"/>
        </span>
      </q-item>

    </div>
    <div class="clear"></div>
  </div>
</template>

<script>

  export default {
    name: 'Post',
    props: {
      post: {
        type: Object,
        required: true
      }
    },
    computed: {
      to() {
        return this.category.getMaterialRoute(this.post.id);
      },
      toComments() {
        return this.category.getMaterialRoute(this.post.id, '#comments');
      },
      category() {
        return this.$store.getters.getCategory(this.post.categoryName);
      }
    },
    methods: {
      prepareLocalLinks() {
        const el = this.$el.getElementsByClassName('post-text')[0];
        const links = el.getElementsByTagName('a');
        for (const link of links) {
          if (link.href.startsWith(config.SiteUrl)) {
            link.addEventListener('click', (e) => {
              const url = link.href.substring(config.SiteUrl.length);
              this.$router.push(url);
              e.preventDefault();
            });
          }
        }
      }
    },
    mounted() {
      this.prepareLocalLinks();
    }
  }

</script>


<style lang="stylus">

  .post {
    .avatar {
      margin-right: 12px;
    }

    .header {
      display: flex;
      padding: 2px 0;
    }

    .blog-title {
      font-weight: 600 !important;
      color: $link-color !important;
    }

    $footer-line-height = 38px;

    .footer {
      align-items: center;
      color: $link-color !important;

      .q-item {

        min-height: unset !important;
        height: $footer-line-height;
      }

      .q-item:first-child {
        padding-left: 0;
      }
    }

    .post-preview {

      margin: 3px 0;

      *:first-child {
        margin-top: 0 !important;
      }

      *:last-child {
        margin-bottom: 0 !important;
      }
    }

    .user-link {
      color: $grey-6;

      &:hover {
        color: $link-color !important;
        text-decoration: underline;
      }
    }

    .date {
      display: flex;
      float: right;
      align-items: center;
      height: $footer-line-height;

      .q-icon {
        margin-right: 7px;
      }
    }
  }

</style>
