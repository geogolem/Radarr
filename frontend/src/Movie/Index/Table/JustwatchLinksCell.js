import PropTypes from 'prop-types';
import React from 'react';
import { icons } from 'Helpers/Props';
import VirtualTableRowCell from 'Components/Table/Cells/TableRowCell';
import styles from './JustwatchLinksCell.css';
import Link from 'Components/Link/Link';

import iconSet from './selection.json'
import IcomoonReact, {iconList} from 'icomoon-react'

function JustwatchLinksCell(props) {
  const {
    className,
    justwatchUrl,
    netflixUrl,
    primeVideoUrl,
	tubiTVUrl,
	hooplaUrl,
    component: Component,
    ...otherProps
  } = props;

  return (


 <Component
      className={className}
      {...otherProps}
    >
    { justwatchUrl &&

      <Link
            className={styles.link}
            to={` ${justwatchUrl}`}
      > 

        <IcomoonReact 
		  iconSet={iconSet} 
		  color="#444" 
		  size="20px" 
		  icon="justwatch"
		  title={justwatchUrl}
         />
      </Link>
    }

    { netflixUrl &&

      <Link
            className={styles.link}
            to={` ${netflixUrl}`}
      > 

        <IcomoonReact
		  iconSet={iconSet}
		  color="#444"
		  icon="netflix"
		  size="20px"
          title={netflixUrl}
         />
      </Link>
    }
    { primeVideoUrl &&

      <Link
            className={styles.link}
            to={` ${primeVideoUrl}`}
      > 

        <IcomoonReact
		  iconSet={iconSet}
		  color="#444"
		  icon="amazon"
		  size="20px"
          title={primeVideoUrl}
         />
      </Link>
    }
    { tubiTVUrl &&

      <Link
            className={styles.link}
            to={` ${tubiTVUrl}`}
      > 

        <IcomoonReact
		  iconSet={iconSet}
		  color="#444"
		  icon="tubi"
		  size="20px"
          title={tubiTVUrl}
         />
      </Link>
    }
    { hooplaUrl &&

      <Link
            className={styles.link}
            to={` ${hooplaUrl}`}
      > 

        <IcomoonReact
		iconSet={iconSet}
		  color="#444"
		  icon="hoopla"
		  size="20px"
          name={hooplaUrl ? icons.MONITORED : icons.UNMONITORED}
          title={hooplaUrl}
         />
      </Link>
    }

  </Component>

  );
}

JustwatchLinksCell.propTypes = {
  className: PropTypes.string.isRequired,
  justwatchUrl: PropTypes.string,
  netflixUrl: PropTypes.string,
  primeVideUrl: PropTypes.string,
  tubiTVUrl: PropTypes.string,
  hooplaUrl: PropTypes.string,
  component: PropTypes.elementType
};

JustwatchLinksCell.defaultProps = {
  className: styles.status,
  component: VirtualTableRowCell
};

export default JustwatchLinksCell;