import {BlogPage} from 'sun'
import {Material} from 'sun'

export default {
  name: 'Blog',
  title: 'Blog',
  categoryType: 'Blog',

  setCategoryRoute(category) {
    category.route = {
      name: `blog-${category.name}`,
      params:  {}
    }
  },

  getRoutes(category) {
    const name = category.name;
    const nameLower = name.toLowerCase();

    return [
      {
        name: `blog-${name}`,
        path: '/' + nameLower,
        components: {
          default: BlogPage,
          navigation: null
        },
        props: {
          default: {
            categoryName: nameLower
          }
        }
      },
      {
        name: `blog-${name}-mat`,
        path: `/${nameLower}/:idOrName`,
        components: {
          default: Material,
          navigation: null
        },
        props: {
          default: (route) => {
            return {
              categoryName: nameLower,
              idOrName: route.params.idOrName
            }
          }
        }
      }
    ]
  }
}
