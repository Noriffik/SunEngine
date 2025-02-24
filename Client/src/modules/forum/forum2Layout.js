import {ForumPanel, NewTopics, Thread} from 'sun'
import {Categories2} from 'sun'
import {Material} from 'sun'

export default {
  name: 'Forum2',
  title: 'Forum 2',
  categoryType: 'Forum',

  setCategoryRoute(category) {
    category.route = {
      name: `forum-${category.name}`,
      params:  {}
    };

    for(const cat0 of category.subCategories) {
      for (const cat1 of cat0.subCategories) {
        cat1.route = {
          name: `forum-${category.name}-cat`,
          params: {
            categoryName: cat1.name
          }
        }
      }
    }
  },

  getRoutes(category) {
    const name = category.name;
    const nameLower = name.toLowerCase();

    return [
      {
        name: `forum-${name}`,
        path: '/' + nameLower,
        components: {
          default: NewTopics,
          navigation: ForumPanel
        },
        props: {
          default: {categoryName: name},
          navigation: {categories: Categories2, categoryName: name}
        },
      },
      {
        name: `forum-${name}-cat`,
        path: `/${nameLower}/:categoryName`,
        components: {
          default: Thread,
          navigation: ForumPanel
        },
        props: {
          default: true,
          navigation: {categories: Categories2, categoryName: name}
        }
      },
      {
        name: `forum-${name}-cat-mat`,
        path: `/${nameLower}/:categoryName/:idOrName`,
        components: {
          default: Material,
          navigation: ForumPanel
        },
        props: {
          default: (route) => {
            return {categoryName: route.params.categoryName, idOrName: route.params.idOrName}
          },
          navigation: {categories: Categories2, categoryName: name}
        }
      }
    ]
  }
}
